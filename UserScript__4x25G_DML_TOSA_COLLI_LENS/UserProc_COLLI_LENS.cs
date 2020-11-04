using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UserScript.CamRAC;
using UserScript.SystemService;

namespace UserScript
{
    partial class APAS_UserScript
    {
        #region Variables

        /// <summary>
        /// 会聚光Lens耦合使用的功率计
        /// </summary>
        const string PM_FOCUS = "PM1906A1";

        /// <summary>
        /// 准直Lens耦合使用的功率计
        /// </summary>
        const string PM_COLLI = "PM1906A2";

        /// <summary>
        /// 耦合后最小光功率
        /// </summary>
        const double TARGET_POWER_MIN_DBM = 0;

        /// <summary>
        /// 耦合后最大光功率
        /// </summary>
        const double TARGET_POWER_MAX_DBM = 10;

        #endregion

        #region User Process

        /// <summary>
        /// The section of the user process.
        /// 用户自定义流程函数。
        /// 
        /// Please write your process in the following method.
        /// 请在以下函数中定义您的工艺流程。
        /// 
        /// </summary>
        /// <param name="Apas"></param>
        /// <returns></returns>
        static void UserProc(SystemServiceClient Apas, CamRemoteAccessContractClient Camera = null)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                Stopwatch swTotal = new Stopwatch();
                swTotal.Start();

                // 初始化功率计状态
                Apas.__SSC_LogInfo("初始化光功率计...");
                Apas.__SSC_Powermeter_SetRange(PM_COLLI, SSC_PMRangeEnum.AUTO);
                Apas.__SSC_Powermeter_SetUnit(PM_COLLI, SSC_PMUnitEnum.dBm);

                // 等待功率计稳定
                Thread.Sleep(500);


                // 读取初始功率
                double power = Apas.__SSC_Powermeter_Read(PM_COLLI);
                Apas.__SSC_LogInfo($"初始光 {power:F2}dBm");


                // STEP 1: RECT Area Scan
                if (power < -25)
                {
                    sw.Restart();

                    Step1(Apas);

                    sw.Stop();
                    Apas.__SSC_LogInfo($"耗时: {sw.Elapsed.TotalSeconds:F1}s");
                }


                // Step 2: Fast ND Alignment
                if(power < 0)
                {
                    sw.Restart();

                    Step2(Apas);

                    sw.Stop();
                    Apas.__SSC_LogInfo($"耗时: {sw.Elapsed.TotalSeconds:F1}s");
                }

                // STEP 3: Profile ND to fine-tune.
                sw.Restart();

                Step3(Apas);

                sw.Stop();
                Apas.__SSC_LogInfo($"耗时: {sw.Elapsed.TotalSeconds:F1}s");

                // STEP 4: 双边耦合
                sw.Reset();

                Step4(Apas);

                sw.Stop();
                Apas.__SSC_LogInfo($"耗时: {sw.Elapsed.TotalSeconds:F1}s");


                // STEP 5: Hill Climb
                sw.Restart();

                Step5(Apas);

                sw.Stop();
                Apas.__SSC_LogInfo($"耗时: {sw.Elapsed.TotalSeconds:F1}s");
                Apas.__SSC_LogInfo($"总耗时: {swTotal.Elapsed.TotalSeconds:F1}s");




                // 检查光功率是否达标
                Apas.__SSC_Powermeter_SetRange(PM_COLLI, SSC_PMRangeEnum.AUTO);
                Thread.Sleep(500);

                power = Apas.__SSC_Powermeter_Read(PM_COLLI);
                Apas.__SSC_LogInfo($"最终光功率为 {power:F2}dBm");

                if (TARGET_POWER_MIN_DBM <= power)
                {
                    Apas.__SSC_LogInfo($"脚本运行完成");
                }
                else
                {
                    var msg = $"无法耦合到目标功率 {TARGET_POWER_MIN_DBM}dBm";
                    Apas.__SSC_LogError(msg);
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                Apas.__SSC_LogError(ex.Message);
                throw ex;
            }

            // Thread.Sleep(100);
        }
        #endregion

        #region Private Methods

        static void Step1(SystemServiceClient Service)
        {
            int cycle = 0;

        __redo_rectscan:
            Service.__SSC_LogInfo($"开始面扫描搜索初始光...cycle({cycle})");
            cycle++;

            PerformAlignment(Service, Service.__SSC_DoRectAreaScan, "准直Lens_初始光", SSC_PMRangeEnum.RANGE2, double.NaN, 2);

            Thread.Sleep(500);

            // Service.__SSC_Powermeter_SetRange(PM_CAPTION, SSC_PMRangeEnum.AUTO);
            var power = Service.__SSC_Powermeter_Read(PM_COLLI);
            Service.__SSC_LogInfo($"最大功率：{power:F2}dBm");

            // The exit condition is power > -15dBm
            if (power < -15)
            {
                if (cycle == 1)
                {
                    Service.__SSC_LogWarn($"搜索失败，Z轴前进20um重新搜索...");
                    Service.__SSC_MoveAxis("Lens", "Z", SSC_MoveMode.REL, 100, -20);
                    goto __redo_rectscan;
                }
                else if (cycle == 2)
                {
                    Service.__SSC_LogWarn($"搜索失败，Z轴前进10um重新搜索...");
                    Service.__SSC_MoveAxis("Lens", "Z", SSC_MoveMode.REL, 100, 40);
                    goto __redo_rectscan;
                }
                else
                {
                    throw new Exception("无法找到初始功率， 请检查Lens位置。");
                }
            }
        }

        static void Step2(SystemServiceClient Service)
        {
            Queue<double> powerHistory = new Queue<double>();
            int cycle = 0;

            Service.__SSC_LogInfo("开始执行快速扫描....");

            var range = SSC_PMRangeEnum.RANGE3;

            while (true)
            {
                // XY Scan
                PerformAlignment(Service, Service.__SSC_DoFastND, "准直Lens_XY_0.2_20_Z_1_20", range, 0.2, 10);
                var power = Service.__SSC_Powermeter_Read(PM_COLLI);
                Service.__SSC_LogInfo($"光功率：{power:F2}dBm");

                if (power > 0)
                    break;

                powerHistory.Enqueue(power);
                if (powerHistory.Count > 5)
                    powerHistory.Dequeue();

                if (powerHistory.Count > 2)
                {
                    DataAnalysis.CheckSlope(powerHistory.ToArray(), out DataAnalysis.SlopeTrendEnum trend);
                    Service.__SSC_LogInfo($"功率变化趋势：{trend.ToString()}");

                    if (trend == DataAnalysis.SlopeTrendEnum.Ripple)
                        break;
                }

                cycle++;

                if (cycle > 20)
                    throw new Exception("快速扫描失败，无法找到稳定光功率。");

            }
        }

        static void Step3(SystemServiceClient Service)
        {
            Queue<double> powerHistory = new Queue<double>();
            int cycle = 0;
            string profileName = "准直Lens_XY_0.2_10_Z_0.5_10";

            Service.__SSC_Powermeter_SetRange(PM_COLLI, SSC_PMRangeEnum.AUTO);

            Thread.Sleep(500);

            Service.__SSC_LogInfo("开始执行线性扫描....");

            while (true)
            {
                // PowerMeterAutoRange(Service, PM_CAPTION);

                double power = Service.__SSC_Powermeter_Read(PM_COLLI);
                double lastPower = power;

                Service.__SSC_DoProfileND(profileName);

                Thread.Sleep(200);

                power = Service.__SSC_Powermeter_Read(PM_COLLI);
                Service.__SSC_LogInfo($"光功率：{power:F2}dBm");

                //powerHistory.Enqueue(power);
                //if (powerHistory.Count > 5)
                //    powerHistory.Dequeue();

                //if (powerHistory.Count > 2)
                //{
                //    DataAnalysis.CheckSlope(powerHistory.ToArray(), out DataAnalysis.SlopeTrendEnum trend);
                //    Service.__SSC_LogInfo($"功率变化趋势：{trend.ToString()}");

                //    if (trend == DataAnalysis.SlopeTrendEnum.Ripple)
                //    {
                //        break;
                //    }
                //}

                var powerDiff = power - lastPower;
                Service.__SSC_LogInfo($"Power Diff: {powerDiff:F2}dB, {power:F2}dBm/{lastPower:F2}dBm");
                lastPower = power;
                //if (power > 0 && (powerDiff > -0.2 && powerDiff < 0.2))
                if (powerDiff > -0.1 && powerDiff < 0.2)
                    break;
                else
                {
                    cycle++;

                    if (cycle > 10)
                        throw new Exception("慢速扫描执行失败，无法找到稳定光功率。");
                }
            }
        }

        /// <summary>
        /// Rept和准直Lens同时调整。
        /// </summary>
        /// <param name="Apas"></param>
        static void Step4(SystemServiceClient Apas)
        {
            int cycle = 0;
            double power, powerLast;

            Apas.__SSC_LogInfo("开始Rept和准直Lens双边调整...");

            Apas.__SSC_Powermeter_SetRange(PM_COLLI, SSC_PMRangeEnum.AUTO);
            powerLast = Apas.__SSC_Powermeter_Read(PM_COLLI);

            while (true)
            {
                Apas.__SSC_Powermeter_SetRange(PM_COLLI, SSC_PMRangeEnum.RANGE4);
                Apas.__SSC_DoFastND("准直Rept_XY_5_200");

                Apas.__SSC_Powermeter_SetRange(PM_COLLI, SSC_PMRangeEnum.AUTO);
                Apas.__SSC_DoProfileND("准直Lens_XY_0.2_10_Z_0.5_10");

                Thread.Sleep(200);

                power = Apas.__SSC_Powermeter_Read(PM_COLLI);
                var powerDiff = power - powerLast;

                Apas.__SSC_LogInfo($"Power Diff: {powerDiff:F2}dB, {power:F2}dBm, {powerLast:F2}dBm");

                powerLast = power;
                //if (power > 3.5 && (powerDiff > -0.2 && powerDiff < 0.2))
                if (powerDiff > -0.1 && powerDiff < 0.2)
                {
                    break;
                }
                else
                {
                    cycle++;

                    if (cycle > 10)
                    {
                        var msg = "无法调整稳定功率位置。";
                        Apas.__SSC_LogError(msg);
                        throw new Exception(msg);
                    }
                }
            }
        }

        static void Step5(SystemServiceClient Apas)
        {
            Apas.__SSC_LogInfo("开始执行爬山扫描...");

            try
            {
                Apas.__SSC_Powermeter_SetRange(PM_COLLI, SSC_PMRangeEnum.AUTO);
                Apas.__SSC_DoHillClimb("准直Lens_FineTune");
            }
            catch (Exception)
            {

            }
        }


        static void PerformAlignment(SystemServiceClient Service, Func<string, object> AlignmnetHandler, string Profile, SSC_PMRangeEnum PMRange, double BreakPowerDiff_dBm, int MaxCycle)
        {
            int cycle = 0;
            double currPower = Service.__SSC_Powermeter_Read(PM_COLLI);
            double lastPower = currPower;

            while (true)
            {
                Service.__SSC_Powermeter_SetRange(PM_COLLI, PMRange);

                Thread.Sleep(200);

                AlignmnetHandler(Profile);

                Thread.Sleep(200);

                // 计算耦合后和耦合前的功率差值
                currPower = Service.__SSC_Powermeter_Read(PM_COLLI);
                var diffPower = currPower - lastPower;
                lastPower = currPower;

                if (double.IsNaN(BreakPowerDiff_dBm) == false)
                {
                    if (diffPower <= BreakPowerDiff_dBm)
                    {
                        // if the delta power is less than 2dB, jump out of the loop.
                        break;
                    }
                    else
                    {
                        cycle++;
                        if (cycle > MaxCycle)
                            throw new Exception("初始光太小。");
                    }
                }
                else
                {
                    break;
                }
            }
        }


        #endregion

    }
   
}

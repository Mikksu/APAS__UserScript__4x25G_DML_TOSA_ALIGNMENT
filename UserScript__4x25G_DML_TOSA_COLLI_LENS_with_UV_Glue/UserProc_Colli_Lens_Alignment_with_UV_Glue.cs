using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UserScript.CamRAC;
using UserScript.SystemService;

namespace UserScript
{
    internal partial class APAS_UserScript
    {
        #region User Process

        /// <summary>
        ///     The section of the user process.
        ///     用户自定义流程函数。
        ///     Please write your process in the following method.
        ///     请在以下函数中定义您的工艺流程。
        /// </summary>
        /// <param name="Apas"></param>
        /// <returns></returns>
        private static void UserProc(SystemServiceClient Apas, CamRemoteAccessContractClient Camera = null)
        {
            try
            {
                var sw = new Stopwatch();
                var swTotal = new Stopwatch();

                swTotal.Start();
                sw.Start();
                // STEP 3: Profile-ND
                Step3(Apas);
                sw.Stop();
                Apas.__SSC_LogInfo($"耗时: {sw.Elapsed.TotalSeconds:F1}s");

                sw.Restart();
                // STEP 5: Hill Climb
                Step5(Apas);
                sw.Stop();
                Apas.__SSC_LogInfo($"耗时: {sw.Elapsed.TotalSeconds:F1}s");

                swTotal.Stop();
                Apas.__SSC_LogInfo($"总耗时: {swTotal.Elapsed.TotalSeconds:F1}s");

                // 检查光功率是否达标
                Apas.__SSC_Powermeter_SetRange(PM_COLLI, SSC_PMRangeEnum.AUTO);
                Thread.Sleep(500);

                var power = Apas.__SSC_Powermeter_Read(PM_COLLI);
                Apas.__SSC_LogInfo($"最终光功率为 {power:F2}dBm");

                if (TARGET_POWER_MIN_DBM <= power)
                {
                    Apas.__SSC_LogInfo("脚本运行完成");
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

        #region Variables

        /// <summary>
        ///     会聚光Lens耦合使用的功率计
        /// </summary>
        private const string PM_FOCUS = "PM1906A1";

        /// <summary>
        ///     准直Lens耦合使用的功率计
        /// </summary>
        private const string PM_COLLI = "PM1906A2";

        /// <summary>
        ///     耦合后最小光功率
        /// </summary>
        private const double TARGET_POWER_MIN_DBM = 0;

        /// <summary>
        ///     耦合后最大光功率
        /// </summary>
        private const double TARGET_POWER_MAX_DBM = 10;

        private const string LMC_LENS = "Lens";

        private const string LMC_LENS_AXIS_Z = "Z";

        #endregion

        #region Private Methods

        private static void Step3(SystemServiceClient Service)
        {
            var powerHistory = new Queue<double>();
            var cycle = 0;
            var profileName = "准直Lens_XY_0.2_10_Z_0.5_10";

            Service.__SSC_Powermeter_SetRange(PM_COLLI, SSC_PMRangeEnum.AUTO);

            Thread.Sleep(500);

            Service.__SSC_LogInfo("开始执行线性扫描....");

            while (true)
            {
                // PowerMeterAutoRange(Service, PM_CAPTION);

                var power = Service.__SSC_Powermeter_Read(PM_COLLI);
                var lastPower = power;

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
                {
                    break;
                }

                cycle++;

                if (cycle > 10)
                    throw new Exception("慢速扫描执行失败，无法找到稳定光功率。");
            }
        }

        private static void Step5(SystemServiceClient Apas)
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


        private static void PerformAlignment(SystemServiceClient Service, Func<string, object>[] AlignmentHandlers,
            string[] Profiles, SSC_PMRangeEnum PMRange, double BreakPowerDiff_dBm,
            double BreakPowerMax_dBm = double.MaxValue, int MaxCycle = 20)
        {
            if (AlignmentHandlers.Length != Profiles.Length)
                throw new Exception("Handler和Profile的数量不一致。");

            var cycle = 0;
            var currPower = Service.__SSC_Powermeter_Read(PM_COLLI);
            var lastPower = currPower;

            while (true)
            {
                Service.__SSC_Powermeter_SetRange(PM_COLLI, PMRange);

                Thread.Sleep(200);

                for (var i = 0; i < AlignmentHandlers.Length; i++) AlignmentHandlers[i](Profiles[i]);

                Thread.Sleep(200);

                // 计算耦合后和耦合前的功率差值
                currPower = Service.__SSC_Powermeter_Read(PM_COLLI);
                var diffPower = currPower - lastPower;
                lastPower = currPower;

                if (double.IsNaN(BreakPowerDiff_dBm) == false)
                {
                    if (diffPower <= BreakPowerDiff_dBm || currPower >= BreakPowerMax_dBm)
                    {
                        // if the delta power is less than 2dB, jump out of the loop.
                        break;
                    }

                    cycle++;
                    if (cycle > MaxCycle)
                        throw new Exception("初始光太小。");
                }
                else
                {
                    break;
                }
            }

            // wait until the powermeter is stable.
            Thread.Sleep(500);
        }

        #endregion
    }
}
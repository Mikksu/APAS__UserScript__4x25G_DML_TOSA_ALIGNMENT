using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
        private static void UserProc(SystemServiceClient Apas, CamRemoteAccessContractClient Camera = null,
            Options opts = null)
        {
            try
            {
                Apas.__SSC_LogInfo($"目标功率：{opts.PowerThreTerminate:F2}dBm");

                var sw = new Stopwatch();
                var swTotal = new Stopwatch();
                swTotal.Start();

                // 初始化功率计状态
                Apas.__SSC_LogInfo("初始化光功率计...");
                Apas.__SSC_Powermeter_SetRange(opts.PowerMeterCaption, SSC_PMRangeEnum.AUTO);
                Apas.__SSC_Powermeter_SetUnit(opts.PowerMeterCaption, SSC_PMUnitEnum.dBm);

                // 等待功率计稳定
                Thread.Sleep(500);


                // 读取初始功率
                var power = Apas.__SSC_Powermeter_Read(opts.PowerMeterCaption);
                Apas.__SSC_LogInfo($"初始光 {power:F2}dBm");


                // STEP 1: RECT Area Scan
                if (power < -25)
                {
                    if (opts.IgnoreBlindSearch == false)
                    {
                        sw.Restart();

                        Step1(Apas, opts);

                        sw.Stop();
                        Apas.__SSC_LogInfo($"耗时: {sw.Elapsed.TotalSeconds:F1}s");
                    }
                    else
                    {
                        Apas.__SSC_LogWarn($"忽略BlindSearch，当前功率：{power:F2}dBm");
                    }
                }


                // Step 2: Fast Focus Scan
                if (power < 0)
                {
                    if (opts.IgnoreFastFocusScan == false)
                    {
                        sw.Restart();

                        Step2(Apas, opts);

                        sw.Stop();
                        Apas.__SSC_LogInfo($"耗时: {sw.Elapsed.TotalSeconds:F1}s");
                    }
                    else
                    {
                        Apas.__SSC_LogWarn($"忽略快速焦距扫描，，当前功率：{power:F2}dBm");
                    }
                }

                // STEP 3: Profile ND to fine-tune.

                // 等待功率计稳定
                Thread.Sleep(500);
                power = Apas.__SSC_Powermeter_Read(opts.PowerMeterCaption);
                if (opts.IgnoreLensProfileScan == false && power < opts.PowerThreTerminate)
                {
                    

                    sw.Restart();

                    Step3(Apas, opts);

                    sw.Stop();
                    Apas.__SSC_LogInfo($"耗时: {sw.Elapsed.TotalSeconds:F1}s");
                }
                else
                {
                    Apas.__SSC_LogWarn($"忽略Lens线性Profile扫描，，当前功率：{power:F2}dBm");
                }

                // STEP 4: 双边耦合
                // 等待功率计稳定
                Thread.Sleep(500);
                power = Apas.__SSC_Powermeter_Read(opts.PowerMeterCaption);
                if (opts.IgnoreReceptLensDualScan == false && power < opts.PowerThreTerminate)
                {
                    sw.Restart();

                    Step4(Apas, opts);

                    sw.Stop();
                    Apas.__SSC_LogInfo($"耗时: {sw.Elapsed.TotalSeconds:F1}s");
                }
                else
                {
                    Apas.__SSC_LogWarn($"忽略Receptacle-Lens双边扫描，，当前功率：{power:F2}dBm");
                }


                // STEP 5: 最终位置优化
                // 等待功率计稳定
                Thread.Sleep(500);
                power = Apas.__SSC_Powermeter_Read(opts.PowerMeterCaption);
                if (opts.IgnoreFinalFineTune == false && power < opts.PowerThreTerminate)
                {
                    sw.Restart();

                    Step5(Apas, opts);

                    sw.Stop();
                    Apas.__SSC_LogInfo($"耗时: {sw.Elapsed.TotalSeconds:F1}s");
                }
                else
                {
                    Apas.__SSC_LogWarn($"忽略最终微调，，当前功率：{power:F2}dBm");
                }

                swTotal.Stop();
                Apas.__SSC_LogInfo($"总耗时: {swTotal.Elapsed.TotalSeconds:F1}s");


                // 检查光功率是否达标
                Apas.__SSC_Powermeter_SetRange(opts.PowerMeterCaption, SSC_PMRangeEnum.AUTO);
                Thread.Sleep(500);

                power = Apas.__SSC_Powermeter_Read(opts.PowerMeterCaption);
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
                //Apas.__SSC_LogError(ex.Message);
                throw new Exception(ex.Message);
            }

            // Thread.Sleep(100);
        }

        #endregion

        #region Variables

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

        /// <summary>
        /// 执行忙扫。
        /// </summary>
        /// <param name="Service"></param>
        /// <param name="opts"></param>
        private static void Step1(SystemServiceClient Service, Options opts)
        {
            var cycle = 0;

            var powerPrev = Service.__SSC_Powermeter_Read(opts.PowerMeterCaption);

            __redo_rectscan:
            Service.__SSC_LogInfo($"开始面扫描搜索初始光...cycle({cycle})");
            cycle++;

            //PerformAlignment(Service,
            //    new Func<string, object>[] {Service.__SSC_DoRectAreaScan},
            //    opts,
            //    new[] {opts.ProfileNameBlindSearch},
            //    SSC_PMRangeEnum.RANGE1, double.NaN, 2);

            var ret = Service.__SSC_DoRectAreaScan(opts.ProfileNameBlindSearch);

            Thread.Sleep(200);

            // Service.__SSC_Powermeter_SetRange(PM_CAPTION, SSC_PMRangeEnum.AUTO);
            //var power = Service.__SSC_Powermeter_Read(opts.PowerMeterCaption);
            var power = ret.PeakValue;
            Service.__SSC_LogInfo($"最大功率：{power}mV");

            // The exit condition is power > -15dBm
            //if (power < opts.PowerThreRectAreaScan)
            if (power < 3000)
            {
                if (power - powerPrev > 5)
                {
                    cycle--;
                    powerPrev = power;
                    goto __redo_rectscan;
                }

                if (cycle == 1)
                {
                    Service.__SSC_LogWarn("搜索失败，Z轴-20um重新搜索...");
                    Service.__SSC_MoveAxis("Lens", "Z", SSC_MoveMode.REL, 100, -20);
                    goto __redo_rectscan;
                }

                if (cycle == 2)
                {
                    Service.__SSC_LogWarn("搜索失败，Z轴+40um重新搜索...");
                    Service.__SSC_MoveAxis("Lens", "Z", SSC_MoveMode.REL, 100, 40);
                    goto __redo_rectscan;
                }

                throw new Exception("无法找到初始功率， 请检查Lens位置。");
            }
            else
            {
                Service.__SSC_LogInfo("重新搜索峰值...");
                var maxCycle = 0;
                while (true)
                {
                    ret = Service.__SSC_DoRectAreaScan(opts.ProfileNameBlindSearchSmallStep);
                    if (ret.PeakValue > 3000)
                        break;
                    else
                    {
                        if(maxCycle > 5)
                            throw new Exception("无法找到初始功率， 请检查Lens位置。");
                    }

                    maxCycle++;
                }
            }
        }

        /// <summary>
        ///     Fast Focus Scan.
        /// </summary>
        /// <param name="Service"></param>
        private static void Step2(SystemServiceClient Service, Options opts)
        {
            var powerZHistory = new List<PointF>();
            var powerXYHistory = new Queue<double>();
            var cycle = 0;
            double zMoved = 0, nextZMoveStep = opts.FocusScanStep;

            Service.__SSC_LogInfo("开始执行快速扫描....");

            var range = SSC_PMRangeEnum.RANGE3;
            var originZPos = Service.__SSC_GetAbsPosition(LMC_LENS, LMC_LENS_AXIS_Z);
            var power = Service.__SSC_Powermeter_Read(opts.PowerMeterCaption);

            while (true)
            {
                #region XY Scan

                powerXYHistory.Clear();
                cycle = 0;
                while (true)
                {
                    // XY Scan
                    PerformAlignment(Service,
                        new Func<string, object>[] {Service.__SSC_DoFastND, Service.__SSC_DoFastND},
                        opts,
                        new[] {opts.ProfileNameFocusScanLens, opts.ProfileNameFocusScanRecept},
                        range, 0.2, power, 10);

                    power = Service.__SSC_Powermeter_Read(opts.PowerMeterCaption);
                    Service.__SSC_LogInfo($"光功率：{power:F2}dBm");

                    if (power > opts.PowerThreFocusScan)
                    {
                        Service.__SSC_LogInfo("功率达到阈值，耦合结束。");
                        return;
                    }

                    powerXYHistory.Enqueue(power);
                    if (powerXYHistory.Count > 5)
                        powerXYHistory.Dequeue();

                    if (powerXYHistory.Count > 2)
                    {
                        DataAnalysis.CheckSlope(powerXYHistory.ToArray(), out var trend);
                        Service.__SSC_LogInfo($"功率变化趋势：{trend}");

                        if (trend == DataAnalysis.SlopeTrendEnum.Ripple)
                            break;
                    }

                    cycle++;

                    if (cycle > 20)
                        // throw new Exception("快速扫描失败，无法找到稳定光功率。");
                        break;
                }

                #endregion

                power = Service.__SSC_Powermeter_Read(opts.PowerMeterCaption);
                var currZPos = Service.__SSC_GetAbsPosition(LMC_LENS, LMC_LENS_AXIS_Z);
                Service.__SSC_LogInfo($"XY平面功率: {currZPos - originZPos:F4}um, {power:F2}dBm");

                if (power > 0) // exit if the power > 0dBm
                    break;

                powerZHistory.Add(new PointF((float) currZPos, (float)power));

                if (powerZHistory.Count >= 2)
                {
                    power = powerZHistory[powerZHistory.Count - 1].Y;
                    var lastlastPower = powerZHistory[powerZHistory.Count - 2].Y;
                    var diff = power - lastlastPower;
                    Service.__SSC_LogInfo($"功率差: {diff:F2}dBm/{power:F2}dBm/{lastlastPower:F2}dBm");
                    if (diff < -1)
                    {
                        Service.__SSC_LogInfo("功率降低，开始反向搜索...");
                        nextZMoveStep *= -1;
                        nextZMoveStep /= 2;

                        if (Math.Abs(nextZMoveStep) < opts.FocusScanFinalStep)
                        {
                            Service.__SSC_LogInfo("搜索步进收敛至最小，结束搜索！");
                            break;
                        }
                    }
                }

                Service.__SSC_MoveAxis(LMC_LENS, LMC_LENS_AXIS_Z, SSC_MoveMode.REL, 20, nextZMoveStep);

                // accum the total distance of the Z axis moved,
                // be sure it never move out of the RANGE.
                zMoved += nextZMoveStep;
                if (zMoved > opts.FocusScanRange)
                {
                    Service.__SSC_LogWarn("焦点扫描Z轴范围达到最大值。");
                    break;
                }
            }
        }

        /// <summary>
        /// Lens慢速线性扫描
        /// </summary>
        /// <param name="Service"></param>
        /// <param name="opts"></param>
        private static void Step3(SystemServiceClient Service, Options opts)
        {
            var cycle = 0;

            Service.__SSC_Powermeter_SetRange(opts.PowerMeterCaption, SSC_PMRangeEnum.AUTO);

            Thread.Sleep(500);

            Service.__SSC_LogInfo("开始执行线性扫描....");

            while (true)
            {
                // PowerMeterAutoRange(Service, PM_CAPTION);

                var pBeforeAlign = Service.__SSC_Powermeter_Read(opts.PowerMeterCaption);

                if(opts.UseHillClimbInLensAlign)
                    Service.__SSC_DoHillClimb(opts.ProfileNameLineScanLens);
                else
                    Service.__SSC_DoProfileND(opts.ProfileNameLineScanLens);

                Thread.Sleep(500);

                var pAfterAlign = Service.__SSC_Powermeter_Read(opts.PowerMeterCaption);
                Service.__SSC_LogInfo($"光功率：{pAfterAlign:F2}dBm");

                var powerDiff = pAfterAlign - pBeforeAlign;
                Service.__SSC_LogInfo($"Power Diff: {powerDiff:F2}dB, {pAfterAlign:F2}dBm/{pBeforeAlign:F2}dBm");

                //if (power > 0 && (powerDiff > -0.2 && powerDiff < 0.2))
                if (powerDiff > opts.PowerThreLineScanN && powerDiff < opts.PowerThreLineScanP) break;
                if (pAfterAlign >= opts.PowerThreTerminate) break;

                cycle++;

                if (cycle > 10)
                    throw new Exception("慢速扫描执行失败，无法找到稳定光功率。");
            }
        }

        /// <summary>
        /// Receptacle和准直Lens同时调整。
        /// </summary>
        /// <param name="Apas"></param>
        private static void Step4(SystemServiceClient Apas, Options opts)
        {
            var cycle = 0;

            Apas.__SSC_LogInfo("开始Rept和准直Lens双边调整...");

            Apas.__SSC_Powermeter_SetRange(opts.PowerMeterCaption, SSC_PMRangeEnum.AUTO);
            
            while (true)
            {
                var pBeforeAlign = Apas.__SSC_Powermeter_Read(opts.PowerMeterCaption);

                #region 调整Receptacle

                if (opts.UseProfileNdInReceptLensDualScan == false)
                {
                    Apas.__SSC_Powermeter_SetRange(opts.PowerMeterCaption, SSC_PMRangeEnum.RANGE4);
                    Apas.__SSC_DoFastND(opts.ProfileNameDualLineScanRecept);
                }
                else
                {
                    Apas.__SSC_Powermeter_SetRange(opts.PowerMeterCaption, SSC_PMRangeEnum.AUTO);
                    Apas.__SSC_DoProfileND(opts.ProfileNameDualLineScanRecept);
                }

                #endregion

                Thread.Sleep(200);

                #region 调整Lens

                Apas.__SSC_Powermeter_SetRange(opts.PowerMeterCaption, SSC_PMRangeEnum.AUTO);
                if (opts.UseHillClimbInLensAlign)
                    Apas.__SSC_DoHillClimb(opts.ProfileNameDualLineScanLens);
                else
                    Apas.__SSC_DoProfileND(opts.ProfileNameDualLineScanLens);
                #endregion

                Thread.Sleep(500);

                var pAfterAlign = Apas.__SSC_Powermeter_Read(opts.PowerMeterCaption);
                var pDiff = pAfterAlign - pBeforeAlign;

                Apas.__SSC_LogInfo($"Power Diff: {pDiff:F2}dB, {pAfterAlign:F2}dBm, {pBeforeAlign:F2}dBm");

                //if (power > 3.5 && (powerDiff > -0.2 && powerDiff < 0.2))
                if (pDiff > opts.PowerThreDualLineScanN && pDiff < opts.PowerThreDualLineScanP) break;
                if (pAfterAlign >= opts.PowerThreTerminate) break;
                if (pDiff < -0.3) break; // 如果耦合功率开始变小超过-0.3dBm，则停止扫描

                cycle++;

                if (cycle > 10)
                {
                    var msg = "无法调整稳定功率位置。";
                    Apas.__SSC_LogError(msg);
                    throw new Exception(msg);
                }
            }
        }

        /// <summary>
        /// 执行爬山算法微调
        /// </summary>
        /// <param name="Apas"></param>
        /// <param name="opts"></param>
        private static void Step5(SystemServiceClient Apas, Options opts)
        {
            Apas.__SSC_LogInfo("开始执行最终位置优化...");

            try
            {
                Apas.__SSC_Powermeter_SetRange(opts.PowerMeterCaption, SSC_PMRangeEnum.AUTO);
                if(opts.UseHillClimbInFinalFineTune)
                    Apas.__SSC_DoHillClimb(opts.ProfileNameFinalFineTune);
                else
                    Apas.__SSC_DoProfileND(opts.ProfileNameFinalFineTune);
            }
            catch (Exception)
            {
            }
        }


        private static void PerformAlignment(SystemServiceClient Service, Func<string, object>[] AlignmentHandlers,
            Options opts, string[] Profiles, SSC_PMRangeEnum PMRange, double BreakPowerDiff_dBm,
            double BreakPowerMax_dBm = double.MaxValue, int MaxCycle = 5)
        {
            if (AlignmentHandlers.Length != Profiles.Length)
                throw new Exception("Handler和Profile的数量不一致。");

            var cycle = 0;
            var currPower = Service.__SSC_Powermeter_Read(opts.PowerMeterCaption);
            var lastPower = currPower;

            while (true)
            {
                Service.__SSC_Powermeter_SetRange(opts.PowerMeterCaption, PMRange);

                Thread.Sleep(200);

                for (var i = 0; i < AlignmentHandlers.Length; i++)
                    AlignmentHandlers[i](Profiles[i]);

                Thread.Sleep(200);

                // 计算耦合后和耦合前的功率差值
                currPower = Service.__SSC_Powermeter_Read(opts.PowerMeterCaption);
                var diffPower = currPower - lastPower;
                lastPower = currPower;

                if (double.IsNaN(BreakPowerDiff_dBm) == false)
                {
                    if (diffPower <= BreakPowerDiff_dBm || currPower >= BreakPowerMax_dBm)
                        // if the delta power is less than 2dB, jump out of the loop.
                        break;

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
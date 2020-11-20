using System;
using System.Threading;
using UserScript.SystemService;

namespace UserScript
{
    internal static partial class ApasUserScript
    {
        #region User Process

        /// <summary>
        ///     The section of the user process.
        ///     用户自定义流程函数。
        ///     Please write your process in the following method.
        ///     请在以下函数中定义您的工艺流程。
        /// </summary>
        /// <param name="apas"></param>
        /// <param name="channel"></param>
        /// <param name="iBias"></param>
        /// <returns></returns>
        private static void TurnOn(ISystemService apas, int channel, double iBias)
        {
            string err;
            
            if (channel < 1 || channel > 4)
            {
                err = "通道参数错误，通道值必须为1 - 4。";
                apas.__SSC_LogError(err);
                throw new Exception(err);
            }

            if (iBias < 0 || iBias > 150)
            {
                err = "IBias参数错误，IBias必须为0mA - 150mA。";
                apas.__SSC_LogError(err);
                throw new Exception(err);
            }

            // 打开IBias
            using (var iic = new GY7501.GY7501())
            {
                iic.SetIBias(channel, iBias);
                Thread.Sleep(100);

                iic.EnableTx(channel);
                Thread.Sleep(2000);
            }

            // 检测电流
            var icc2 = apas.__SSC_MeasurableDevice_Read("RIGOL DP800s,CH2电流");
            if (!(icc2 < 0.035)) return;

            // throw exception if ICC2 is too small.
            err = "ICC2电流过小。";
            apas.__SSC_LogError(err);
            throw new Exception(err);
        }

        private static void TurnOff(ISystemService apas, int channel)
        {
            if (channel < 0 || channel > 4)
            {
                var err = "通道参数错误，通道值必须为0 - 4。";
                apas.__SSC_LogError(err);
                throw new Exception(err);
            }

            using (var iic = new GY7501.GY7501())
            {
                if (channel > 0)
                {
                    // turn off the specified channel.
                    iic.DisableTx(channel);
                }
                else
                {
                    // turn off all channels.
                    for (int i = 0; i < 4; i++)
                    {
                        iic.DisableTx(i);
                        Thread.Sleep(100);
                    }
                }
            }
        }

        #endregion
    }
}
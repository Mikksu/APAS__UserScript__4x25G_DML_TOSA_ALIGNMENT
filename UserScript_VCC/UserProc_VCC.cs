using System;
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
            if (string.IsNullOrEmpty(PARAM) || PARAM.ToUpper() != "ON" && PARAM.ToUpper() != "OFF")
            {
                var err = "参数错误，请输入参数[ON]或[OFF]。";
                Apas.__SSC_LogError(err);
                throw new Exception(err);
            }

            if (PARAM == "ON")
            {
                Apas.__SSC_EquipmentPluginControl(DP800_CAPTION, $"{PARAM} 3");

                Thread.Sleep(500);

                Apas.__SSC_EquipmentPluginControl(DP800_CAPTION, $"{PARAM} 2");
                Apas.__SSC_EquipmentPluginControl(DP800_CAPTION, $"{PARAM} 1");

                Thread.Sleep(2000);

                // 检查VCC1和VCC3电流
                var ICC1 = Apas.__SSC_MeasurableDevice_Read(DP800_READ_CURR_CH1);
                var ICC3 = Apas.__SSC_MeasurableDevice_Read(DP800_READ_CURR_CH3);

                if (ICC1 >= 0.05 && ICC3 >= 0.008)
                {
                }
                else
                {
                    var err = "ICC1或ICC3未达到指定值。";
                    Apas.__SSC_LogError(err);
                    throw new Exception(err);
                }
            }
            else
            {
                Apas.__SSC_EquipmentPluginControl(DP800_CAPTION, $"{PARAM} 1");
                Apas.__SSC_EquipmentPluginControl(DP800_CAPTION, $"{PARAM} 2");


                Thread.Sleep(500);

                Apas.__SSC_EquipmentPluginControl(DP800_CAPTION, $"{PARAM} 3");
            }
        }

        #endregion

        #region Variables

        /// <summary>
        ///     DP800名称
        /// </summary>
        private const string DP800_CAPTION = "RIGOL DP800s";

        private const string DP800_READ_CURR_CH1 = "RIGOL DP800s,CH1电流";
        private const string DP800_READ_CURR_CH2 = "RIGOL DP800s,CH2电流";
        private const string DP800_READ_CURR_CH3 = "RIGOL DP800s,CH3电流";

        #endregion

        #region Private Methods

        #endregion
    }
}
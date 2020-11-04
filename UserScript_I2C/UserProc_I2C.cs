using System;
using System.Threading;
using UserScript.CamRAC;
using UserScript.SystemService;

namespace UserScript
{
    partial class APAS_UserScript
    {
        #region Variables

        /// <summary>
        /// DP800名称
        /// </summary>
        const string DP800_CAPTION = "RIGOL DP800s";

        const string DP800_READ_CURR_CH1 = "RIGOL DP800s,CH1电流";
        const string DP800_READ_CURR_CH2 = "RIGOL DP800s,CH2电流";
        const string DP800_READ_CURR_CH3 = "RIGOL DP800s,CH3电流";

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
            double ibias = 0;
            int channel = 0;
            try
            {
                if(int.TryParse(PARAM_CH, out channel) == false)
                {
                    var err = "参数[2]错误，通道必须为数字。";
                    Apas.__SSC_LogError(err);
                    throw new Exception(err);
                }

                if(channel < 1 || channel > 4)
                {
                    var err = "参数[2]错误，通道值范围必须为1 - 4。";
                    Apas.__SSC_LogError(err);
                    throw new Exception(err);
                }

                if (PARAM_FUNC == "ON")
                {
                    if(double.TryParse(PARAM_IBIAS, out ibias) == false)
                    {
                        var err = "参数[3]错误，IBias必须为数字。";
                        Apas.__SSC_LogError(err);
                        throw new Exception(err);
                    }

                    // 打开IBias
                    var iic = new GY7501.GY7501();
                    
                    iic.SetIbias(channel, ibias);
                    Thread.Sleep(100);

                    iic.EnableTx(channel);
                    Thread.Sleep(2000);

                    // 检测电流
                    var icc2 = Apas.__SSC_MeasurableDevice_Read("RIGOL DP800s,CH2电流");
                    if(icc2 < 0.035)
                    {
                        var err = "ICC2电流过小。";
                        Apas.__SSC_LogError(err);
                        throw new Exception(err);
                    }
                }
                else if(PARAM_FUNC == "OFF")
                {
                    var iic = new GY7501.GY7501();
                    iic.DisableTx(channel);
                }
                else
                {
                    var err = "参数[1]错误，仅支持ON和OFF。";
                    Apas.__SSC_LogError(err);
                    throw new Exception(err);
                }
            }
            catch(Exception ex)
            {
                throw;
            }

            


        }
        #endregion

        #region Private Methods

        #endregion

    }
}

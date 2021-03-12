using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        /// <param name="apas"></param>
        /// <param name="camera"></param>
        /// <param name="opts"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        private static void UserProc(ISystemService apas, CamRemoteAccessContractClient camera = null,
            Option opts = null)
        {
            if (opts == null) throw new ArgumentException(nameof(opts));

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var fullname = $"{desktopPath}\\{opts.FilenamePrefix}_{DateTime.Now:yyyyMMdd}.csv";

            var records = new List<AlignmentData>();
            var data = new AlignmentData();
            data.Sn = ReadVariable<string>(apas.__SSC_ReadVariable, "__SN");
            data.Pn = ReadVariable<string>(apas.__SSC_ReadVariable, "__PN");
            data.Traveler = ReadVariable<string>(apas.__SSC_ReadVariable, "__TC");
            data.WorkOrder = ReadVariable<string>(apas.__SSC_ReadVariable, "__WO");
            data.Operator = ReadVariable<string>(apas.__SSC_ReadVariable, "__OP");
            data.LDLensGap_CH3 = ReadVariable<double>(apas.__SSC_ReadVariable, "LD_LENS_GAP_CH3", double.Parse);
            data.LDLensGap_CH0 = ReadVariable<double>(apas.__SSC_ReadVariable, "LD_LENS_GAP_CH0", double.Parse);
            data.LDLensPowerAfterAlignment = ReadVariable<double>(apas.__SSC_ReadVariable, "MAX_POWER_CH0", double.Parse);
            data.LDLensPowerBeforeUVCuring = ReadVariable<double>(apas.__SSC_ReadVariable, "P_LD_LENS_BEFORE_UV", double.Parse);
            data.LDLensPowerAfterUVCuring = ReadVariable<double>(apas.__SSC_ReadVariable, "P_LD_LENS_AFTER_UV", double.Parse);
            data.FiberLensGap = ReadVariable<double>(apas.__SSC_ReadVariable, "FIBER_LENS_GAP", double.Parse);
            data.FiberLensPowerAfterAlignment = ReadVariable<double>(apas.__SSC_ReadVariable, "P_FIB_LENS_POWER", double.Parse);
            data.FiberLensPowerBeforeUV = ReadVariable<double>(apas.__SSC_ReadVariable, "P_FIB_LENS_BEFORE_UV", double.Parse);
            data.FiberLensPowerAfterUV = ReadVariable<double>(apas.__SSC_ReadVariable, "P_FIB_LENS_AFTER_UV", double.Parse);

            data.Time = DateTime.Now;

            records.Add(data);

            bool hasHeader = false;

            if (File.Exists(fullname) == false)
                hasHeader = true;
           
            using (var writer = new StreamWriter(fullname, append:true))
{
                using (var csv = new CsvWriter(writer,
                    new CsvConfiguration(cultureInfo: CultureInfo.InvariantCulture, hasHeaderRecord: hasHeader)))
                {
                    csv.WriteRecords(records);
                }
            }
        }

        #endregion

        #region Private Methods

        private static T ReadVariable<T>(Func<string, object> func, string varName, Func<string, T> typeParser = null)
        {
            try
            {
                var ret = func(varName);
                if (ret == null)
                    return default;

                if (typeParser == null)
                   return (T)ret;
                else
                    return typeParser(ret.ToString());
            }
            catch (Exception)
            {
                //throw new InvalidOperationException($"无法读取变量 [{varName}]({ret})，{ex.Message}");
                return default;
            }
        }

        #endregion
    }
}
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
            data.Op = ReadVariable<string>(apas.__SSC_ReadVariable, "__OP");
            data.LDLensPowerBeforeUV = ReadVariable<double>(apas.__SSC_ReadVariable, "POWER_BEFORE_UV", double.Parse);
            data.LDLensPowerAfterUV = ReadVariable<double>(apas.__SSC_ReadVariable, "POWER_AFTER_UV", double.Parse);
            data.LDLensGap = ReadVariable<double>(apas.__SSC_ReadVariable, "LENS_GAP", double.Parse);
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
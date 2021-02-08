using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
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
            var fullname = $"{desktopPath}\\{opts.Filename}_{DateTime.Now:yyyyMMdd}.csv";

            var records = new List<AlignmentData>();
            var data = new AlignmentData
            {
                LensType = opts.LensType,
                Sn = ReadVariable<string>(apas.__SSC_ReadVariable, "_SN"),
                Pn = ReadVariable<string>(apas.__SSC_ReadVariable, "_PN"),
                Traveler = ReadVariable<string>(apas.__SSC_ReadVariable, "_TC"),
                WorkOrder = ReadVariable<string>(apas.__SSC_ReadVariable, "_WO"),
                Op = ReadVariable<string>(apas.__SSC_ReadVariable, "__OP"),
                PowerBeforeUV = ReadVariable<double>(apas.__SSC_ReadVariable, "POWER_BEFORE_UV", double.Parse),
                PowerAfterUV = ReadVariable<double>(apas.__SSC_ReadVariable, "POWER_AFTER_UV", double.Parse),
                LensGap = ReadVariable<double>(apas.__SSC_ReadVariable, "LENS_GAP", double.Parse),
                Time = DateTime.Now
            };



            records.Add(data);

            using (var writer = new StreamWriter(fullname))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(records);
                }
            }

        }

        #endregion

        #region Private Methods

        private static T ReadVariable<T>(Func<string, object> func, string varName, Func<string, T> typeParser = null)
        {
            object ret = default;
            try
            {
               ret = func(varName);
               if (typeParser == null)
                   return (T)ret;
               else
                   return typeParser(ret.ToString());
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"无法读取变量 [{varName}]({ret})，{ex.Message}");
            }
        }



        #endregion
    }
}
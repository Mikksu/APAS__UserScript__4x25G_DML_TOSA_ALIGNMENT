using System;
using System.Linq;
using CommandLine;
using UserScript.SystemService;

namespace UserScript
{
    /// <summary>
    /// =========================== ATTENTION ===========================
    /// ===========================    注意   =========================== 
    /// =                                                               =  
    /// =          Please DO NOT make ANY changes to this file.         =
    /// =                    请勿修改当前文件的任何内容。                   =
    /// =                                                               =
    /// =================================================================
    /// 
    /// </summary>
    /// 
    internal partial class APAS_UserScript
    {
        private static void Main(string[] args)
        {
            try
            {
                // connect to the APAS.
                var client = new SystemServiceClient();
                client.Open();

                Parser.Default.ParseArguments<CalRYOptions, CalRXOptions>(args)
                    .MapResult(
                      (CalRYOptions opts) =>
                      {
                          UserProc(null, opts: opts);
                          return 0;
                      },
                      (CalRXOptions opts) =>
                      {
                          UserProc(null, opts: opts);
                          return 0;
                      },
                      errs =>
                      {
                               var errmsg = "脚本启动参数错误。";

                               client.__SSC_LogError(errmsg);

                               throw new Exception(errmsg);
                      });
            }
            catch (AggregateException ae)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.Red;

                var ex = ae.Flatten();

                ex.InnerExceptions.ToList().ForEach(e =>
                {
                    Console.WriteLine($"Error occurred, {e.Message}");
                });

                Console.ResetColor();
            }
            //Console.WriteLine("Press any key to exit.");

            //Console.ReadKey();
        }
    }
}

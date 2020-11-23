using System;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
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

                var helpText = new StringBuilder();
                var stream = new StringWriter(helpText);
                var parser = new Parser(config =>
                {
                    config.EnableDashDash = true;
                    config.CaseSensitive = false;
                    config.HelpWriter = stream;
                });

                parser.ParseArguments<CalRYOptions, CalRXOptions>(args)
                    .MapResult(
                        (CalRYOptions opts) =>
                        {
                            UserProc(client, opts: opts);
                            return 0;
                        },
                        (CalRXOptions opts) =>
                        {
                            UserProc(client, opts: opts);
                            return 0;
                        },
                        errs =>
                        {
                            var errmsg = "";

                            if (errs.IsHelp() || errs.IsVersion())
                                errmsg = "";
                            else
                                errmsg = "脚本启动参数错误。\r\n";
                            
                            client.__SSC_LogError(errmsg + helpText.ToString());
                            throw new Exception(errmsg + helpText.ToString());
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

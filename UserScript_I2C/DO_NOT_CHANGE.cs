using System;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using UserScript.SystemService;

namespace UserScript
{
    /// <summary>
    ///     =========================== ATTENTION ===========================
    ///     ===========================    注意   ===========================
    ///     =                                                               =
    ///     =          Please DO NOT make ANY changes to this file.         =
    ///     =                    请勿修改当前文件的任何内容。                   =
    ///     =                                                               =
    ///     =================================================================
    /// </summary>
    internal static partial class ApasUserScript
    {
        private static void Main(string[] args)
        {
            var client = new SystemServiceClient();
            var helpText = new StringBuilder();

            try
            {
                client.Open();

                client.__SSC_Connect();

                var helpWriter = new StringWriter();
                var parser = new Parser(with =>
                {
                    with.CaseSensitive = false;
                    with.EnableDashDash = true;
                    with.HelpWriter = helpWriter;
                });

                parser.ParseArguments<TurnOnOptions, TurnOffOptions>(args)
                    .MapResult(
                        (TurnOnOptions opts) =>
                        {
                            TurnOn(client, opts.Channel, opts.IBias);
                            return 0;
                        },
                        (TurnOffOptions opts) =>
                        {
                            TurnOff(client, opts.Channel);
                            return 0;
                        },
                        errs =>
                        {
                            var myErr = "";
                            if(errs.IsHelp() || errs.IsVersion())
                                myErr =  helpWriter.ToString();
                            else
                            {
                                myErr = $"启动参数错误。\r\n{helpWriter}";
                            }

                            client.__SSC_LogError(myErr);

                            throw new Exception(myErr);
                        });

                client.__SSC_Disconnect();
            }
            catch (AggregateException ae)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.Red;

                var ex = ae.Flatten();

                ex.InnerExceptions.ToList().ForEach(e => { Console.WriteLine($"Error occurred, {e.Message}"); });

                Console.ResetColor();
            }
            finally
            {
                client.Close();
            }
            //Console.WriteLine("Press any key to exit.");

            //Console.ReadKey();
        }
    }
}
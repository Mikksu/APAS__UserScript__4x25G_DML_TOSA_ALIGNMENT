using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    internal partial class APAS_UserScript
    {
        private static void Main(string[] args)
        {
            var client = new SystemServiceClient();

            try
            {
                //client.Open();

                //client.__SSC_Connect();

                var helpWriter = new StringWriter();
                var parser = new Parser(with => with.HelpWriter = helpWriter);

                var parserResult = parser.ParseArguments<Options>(args);
                parserResult.WithParsed(opts =>
                    {
                        //if (opts.IsHelpTextRequired)
                        //{
                        //    var helpText = HelpText.AutoBuild(parserResult, h =>
                        //    {
                        //        h.AutoHelp = false;     // hides --help
                        //        h.AutoVersion = false;  // hides --version
                        //        return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                        //    }, e => e);
                        //    Console.WriteLine(helpText);
                        //}
                        //else
                        {
                            // perform the user process.
                            UserProc(client, opts: opts);
                        }
                    })
                    .WithNotParsed(errs =>
                    {
                        DisplayHelp(errs, helpWriter);

                        var err = "解析启动参数错误。";
                        client.__SSC_LogError(err);
                        throw new Exception(err);
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


        private static void DisplayHelp(IEnumerable<Error> errs, TextWriter helpWriter)
        {
            if (errs.IsVersion() || errs.IsHelp())
                Console.WriteLine(helpWriter.ToString());
            else
                Console.Error.WriteLine(helpWriter.ToString());
        }
    }
}
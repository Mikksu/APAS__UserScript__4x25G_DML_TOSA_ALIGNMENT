using System;
using System.Linq;
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
        private static string PARAM = "";

        private static void Main(string[] args)
        {
            var client = new SystemServiceClient();

            try
            {
                client.Open();

                client.__SSC_Connect();

                if (args.Length != 1)
                {
                    var err = "未指定参数，或参数格式错误。";
                    client.__SSC_LogError(err);
                    throw new Exception(err);
                }

                PARAM = args[0];

                // perform the user process.
                UserProc(client);

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
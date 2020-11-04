using System;
using System.Linq;
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
    partial class APAS_UserScript
    {
        /// <summary>
        /// parameter define:
        /// ON [1-4] [50], 
        /// OFF [1-4]
        /// </summary>
        static string PARAM_FUNC = "";
        static string PARAM_CH = "";
        static string PARAM_IBIAS = "";

        static void Main(string[] args)
        {
            var client = new SystemServiceClient();

            try
            {
                client.Open();

                client.__SSC_Connect();

                if (args.Length < 2 || args.Length > 3)
                {
                    var err = "未指定参数，或参数数量错误。";
                    client.__SSC_LogError(err);
                    throw new Exception(err);
                }

                PARAM_FUNC = args[0];
                PARAM_CH = args[1];

                if (args.Length == 3)
                    PARAM_IBIAS = args[2];

                // perform the user process.
                UserProc(client);

                client.__SSC_Disonnect();
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
            catch(Exception)
            {
                throw;
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

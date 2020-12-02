using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
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
            var isExceptionThrown = false;
            var errText = "";
            var wcfClient = new SystemServiceClient();

            try
            {
                wcfClient.Open();
                wcfClient.__SSC_Connect();

                // print the script version
                wcfClient.__SSC_LogInfo($"Script Version: v{Assembly.GetExecutingAssembly().GetName().Version}");

                if (args.Length != 1)
                {
                    var err = "未指定参数，或参数格式错误。";
                    throw new Exception(err);
                }

                PARAM = args[0];

                // perform the user process.
                UserProc(wcfClient);

                wcfClient.__SSC_Disconnect();
            }
            catch (AggregateException ae)
            {
                var ex = ae.Flatten();
                ex.InnerExceptions.ToList().ForEach(e =>
                {
                    errText = ex.Message;
                    Console.Error.WriteLine(errText);
                });
                isExceptionThrown = true;
            }
            catch (TimeoutException timeProblem)
            {
                errText = "The service operation timed out. " + timeProblem.Message;
                Console.Error.WriteLine(errText);
            }
            // Catch unrecognized faults. This handler receives exceptions thrown by WCF
            // services when ServiceDebugBehavior.IncludeExceptionDetailInFaults
            // is set to true.
            catch (FaultException faultEx)
            {
                errText = "An unknown exception was received. "
                          + faultEx.Message
                          + faultEx.StackTrace;
                Console.Error.WriteLine(errText);
            }
            // Standard communication fault handler.
            catch (CommunicationException commProblem)
            {
                errText = "There was a communication problem. " + commProblem.Message + commProblem.StackTrace;
                Console.Error.WriteLine(errText);
            }
            catch (Exception ex)
            {
                errText = ex.Message;
                Console.Error.WriteLine(errText);
                isExceptionThrown = true;
            }
            finally
            {
                wcfClient.Abort();
                if (isExceptionThrown)
                {
                    // try to output the error message to the log.
                    try
                    {
                        using (wcfClient = new SystemServiceClient())
                        {
                            wcfClient.__SSC_LogError(errText);
                            wcfClient.Abort();
                        }
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                   

                    Environment.ExitCode = -1;
                }
            }
        }
    }
}
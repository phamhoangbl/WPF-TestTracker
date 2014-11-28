using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using TestTracker.Core.Data.Model;
using TestTracker.Core.Data.Repository;
using TestTracker.Core.Utils;
using NLog;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace TestTracker.ConsoleApp
{
    public class Program
    {
        #region Const Strings

        private const string STR_ALL_NETWORK_ARE_BUSY_TITLE = "Connection refused:";
        private const string STR_NETWORK_ARE_FAIL_TITLE = "Connection failed:";
        private const string STR_WRONG_HBA_CONFIGURATION_TITLE = "DriveMaster HBA Configuration";
        private const string STR_WRONG_PORT_TITLE = "Please check if the DEVICE on the SELECTED port works well!";
        //private const string STR_DRIVE_MASTER_TITLE = "DriveMaster";
        private const string STR_DRIVE_MASTER_TITLE = "caption";
        private const string STR_DMTEST_PATH = @"C:\DMTest";
        private const string STR_DMTEST_SVN_PATH = @"C:\DMTestSVN";
        private const string _STR_LOG_MESSAGE = "Moved file excels and logs";

        #endregion


        static Logger _logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            var options = new Options();
            var parser = new CommandLine.Parser(with => with.HelpWriter = Console.Error);

            if (parser.ParseArgumentsStrict(args, options, () => Environment.Exit(-2)))
            {
                Run(options);
            }
        }

        private static void Run(Options options)
        {
            string testQueueId = options.TestQueueId;
            string filePatch = options.FilePath;
            string scriptName = options.ScriptName;
            string verdorId = options.VerdorId;
            string deviceId = options.DeviceId;
            string port = options.Port;
            string otherOption = options.OtherOption;

            string textDebug = string.Empty;
            textDebug += "---------------Processing--------------";
            textDebug += String.Format("Test Queue Id: {0}", options.TestQueueId) + " \n";
            textDebug += String.Format("File path: {0}", options.FilePath) + " \n";
            textDebug += String.Format("Script Name: {0}", options.ScriptName) + " \n";
            textDebug += String.Format("Verdor Id: {0}", options.VerdorId) + " \n";
            textDebug += String.Format("Device Id: {0}", options.DeviceId) + " \n";
            textDebug += String.Format("Port: {0}", options.Port) + " \n";
            textDebug += String.Format("Other Options: {0}", options.OtherOption) + " \n";

            _logger.Info(textDebug);

            var testQueueRepository = new TestQueueRepository();
            try
            {
                int result;
                CallDmMaster(testQueueId, filePatch, scriptName, verdorId, deviceId, port, otherOption, out result);
                //Update status when have done run DM Master
                if (result == 0)
                {
                    FileExport file = new FileExport();
                    List<TestUnitResult> testUnitResultList;
                    List<TestResult> listFileResult = file.CheckFileResult(out testUnitResultList);

                    //check exported file
                    if (listFileResult.Any())
                    {
                        var firmwareRevision = listFileResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.FWRevision));
                        var modelNumber = listFileResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.ModelNumber));
                        var serialNumber = listFileResult.FirstOrDefault(x => !string.IsNullOrEmpty(x.SerialNumber));

                        //if file export doesn't have any firmwareRevision, modelNumber, or serialNUmber, return umcompleted;
                        if(firmwareRevision == null || modelNumber == null || serialNumber == null)
                        {
                            testQueueRepository.UpdateTestQueueStatus(int.Parse(testQueueId), EnumTestStatus.Uncompleted);
                            textDebug += string.Format("Not found firmwareRevision, modelNumber, or serialNUmber", scriptName);
                        }

                        string repoSVNConnectionString = System.Configuration.ConfigurationManager.AppSettings["RepoSVNConnectionString"];
                        string usernameSVN = System.Configuration.ConfigurationManager.AppSettings["usernameSVN"];
                        string userPasswordSVN = System.Configuration.ConfigurationManager.AppSettings["userPasswordSVN"];

                        string errorMessage;
                        bool isUploaded = SvnSharpClient.UploadFile(repoSVNConnectionString, usernameSVN, userPasswordSVN, STR_DMTEST_PATH, STR_DMTEST_SVN_PATH, firmwareRevision.FWRevision.Trim(), modelNumber.ModelNumber.Trim(), serialNumber.SerialNumber.Trim(), string.Format("Add File excels and logs, script name: {0}, {1}", scriptName, DateTime.UtcNow.ToString("f")), out errorMessage);

                        if (isUploaded)
                        {
                            //if done to upload file, check and store result to dataabase.
                            var testResultRepository = new TestResultRepository();
                            var testUnitResultRepository = new TestUnitResultRepository();
                            foreach(var fileResult in listFileResult)
                            {
                                fileResult.TestQueueId = int.Parse(testQueueId);
                                fileResult.CreatedDateUtc = DateTime.UtcNow;
                                testResultRepository.InsertTestResult(fileResult);
                            }

                            foreach(var testUnitResult in testUnitResultList)
                            {
                                testUnitResult.TestQueueId = int.Parse(testQueueId);
                                testUnitResult.CreatedDateUtc = DateTime.UtcNow;
                                testUnitResultRepository.InsertTestUnitResult(testUnitResult);
                            }

                            testQueueRepository.UpdateTestQueueStatus(int.Parse(testQueueId), EnumTestStatus.Completed);
                            textDebug += string.Format("Done: Updated Status Queue is completed for {0}***", scriptName);
                        }
                        else
                        {
                            //Fails to connect VPN
                            if (errorMessage.Contains("Unable to connect to a repository at URL"))
                            {
                                testQueueRepository.UpdateTestQueueStatus(int.Parse(testQueueId), EnumTestStatus.FailConnection);
                                textDebug += string.Format("Not Done: Upload file has problems: {0}", errorMessage);
                            }
                            else
                            {
                                testQueueRepository.UpdateTestQueueStatus(int.Parse(testQueueId), EnumTestStatus.FailConnection);
                                textDebug += string.Format("Not Done: Upload file has problems: {0}", errorMessage);
                            }
                        }
                    }
                    //if not, change status = UmCompleted
                    else
                    {
                        testQueueRepository.UpdateTestQueueStatus(int.Parse(testQueueId), EnumTestStatus.Uncompleted);
                        textDebug += string.Format("Not Done: Updated Status Queue is not completed for {0}***", scriptName);
                    }
                }
                else if(result == 1)
                {
                    testQueueRepository.UpdateTestQueueStatus(int.Parse(testQueueId), EnumTestStatus.BusyConnection);
                    textDebug += "Not Done: All lienses are busy";
                }
                else if (result == 2)
                {
                    testQueueRepository.UpdateTestQueueStatus(int.Parse(testQueueId), EnumTestStatus.FailConnection);
                    textDebug += "Not Done: Connection fails, check your VPN";
                }
                else if (result == 3)
                {
                    testQueueRepository.UpdateTestQueueStatus(int.Parse(testQueueId), EnumTestStatus.WrongHBAConfig);
                    textDebug += "Not Done: Wrong HBA Config, check device configuration";
                }
                else if (result == 4)
                {
                    testQueueRepository.UpdateTestQueueStatus(int.Parse(testQueueId), EnumTestStatus.WrongPort);
                    textDebug += "Not Done: Wrong Port, please check if the device on the selected port works well";
                }

                textDebug += "---------------------------------------";
                _logger.Info(textDebug);
            }
            catch(Exception ex)
            {
                _logger.Error(string.Format("Error when trying to update status: {0}", ex.Message));
                testQueueRepository.UpdateTestQueueStatus(int.Parse(testQueueId), EnumTestStatus.Uncompleted);
            }

        }

        //result = 0 ---> complete
        //result = 1 ---> uncompleted because of all net work liense DM are busy
        private static void CallDmMaster(string testQueueId, string filePatch, string scriptName, string verdorId, string deviceId, string port, string orderOption, out int result)
        {
            result = 0;
            // Run Multi script Setup the process with the ProcessStartInfo class.
            // Retrieve Queues have status = Running

            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.UseShellExecute = false;
            startinfo.CreateNoWindow = true;
            startinfo.WindowStyle = ProcessWindowStyle.Hidden;
            startinfo.FileName = filePatch;
            startinfo.RedirectStandardOutput = true;
            string arguments = string.Format(@"/s:{0} /v:{1} /D:{2} /P:{3} /l:/e", scriptName, verdorId, deviceId, port);
            startinfo.Arguments = arguments;
            _logger.Info(string.Format("File patch: {0}, arguments: {1}", filePatch, arguments));

            //Assign Procesing status
            Process.Start(startinfo);
            //wait for 25 s to DM process start
            Thread.Sleep(1000);

            _logger.Info(string.Format(string.Format("Done call DM Master app", DateTime.UtcNow)));

            Process[] processlist = Process.GetProcesses();

            foreach (Process theprocess in processlist)
            {
                //_logger.Info(string.Format(string.Format("Processes: {0}, {1}", theprocess.ProcessName, theprocess.MainWindowTitle)));
                if (theprocess.MainWindowTitle == STR_ALL_NETWORK_ARE_BUSY_TITLE)
                {
                    _logger.Info(string.Format(string.Format("All net work liense DM are busy {0}", DateTime.UtcNow)));
                    theprocess.Kill();
                    result = 1;
                }
                if (theprocess.MainWindowTitle == STR_NETWORK_ARE_FAIL_TITLE)
                {
                    _logger.Info(string.Format(string.Format("Failed to connect VPN {0}", DateTime.UtcNow)));
                    theprocess.Kill();
                    result = 2;
                }
                if (theprocess.MainWindowTitle.Contains(STR_WRONG_HBA_CONFIGURATION_TITLE))
                {
                    _logger.Info(string.Format(string.Format("Wrong HBA Configuration {0}", DateTime.UtcNow)));
                    theprocess.Kill();
                    result = 3;
                }
                if (theprocess.MainWindowTitle.Contains(STR_WRONG_PORT_TITLE))
                {
                    _logger.Info(string.Format(string.Format("Wrong HBA Configuration {0}", DateTime.UtcNow)));
                    theprocess.Kill();
                    result = 4;
                }
                
            }

            //check if DM processing is done or yet
            bool isFinished = false;
            while (!isFinished)
            {
                Process[] processes = Process.GetProcesses();
                if (!processes.Any(x => x.MainWindowTitle.Contains(STR_DRIVE_MASTER_TITLE)))
                {
                    isFinished = true;
                }
            }
        }
    }
}

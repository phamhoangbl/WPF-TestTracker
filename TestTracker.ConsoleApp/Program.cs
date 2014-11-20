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
        private const string ALL_NETWORK_ARE_BUSY_TITLE = "Connection refused:";
        private const string ALL_NETWORK_ARE_FAIL_TITLE = "Connection failed:";
        static Logger _logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            var options = new Options();
            var parser = new CommandLine.Parser(with => with.HelpWriter = Console.Error);

            if (parser.ParseArgumentsStrict(args, options, () => Environment.Exit(-2)))
            {
                Run(options);
            }

            Console.ReadLine();
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
                    //check exported file
                    //if has files

                    testQueueRepository.UpdateStatus(int.Parse(testQueueId), EnumTestStatus.Completed);

                    //if not
                    //testQueueRepository.UpdateStatus(int.Parse(testQueueId), EnumTestStatus.Uncompleted);
                    textDebug += string.Format("Done: Updated Status Queue is completed for {0}***", scriptName);
                }
                else if(result == 1)
                {
                    testQueueRepository.UpdateStatus(int.Parse(testQueueId), EnumTestStatus.BusyConnection);
                    textDebug += "Not Done: All lienses are busy";
                }
                else if (result == 2)
                {
                    testQueueRepository.UpdateStatus(int.Parse(testQueueId), EnumTestStatus.FailConnection);
                    textDebug += "Not Done: Connection fails, check your VPN";
                }
                textDebug += "---------------------------------------";
                _logger.Info(textDebug);
            }
            catch(Exception ex)
            {
                _logger.Info(string.Format("Error when trying to update status: {0}", ex.Message));
                testQueueRepository.UpdateStatus(int.Parse(testQueueId), EnumTestStatus.Uncompleted);
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
            string arguments = string.Format(@"/s:""{0}"" /v:""{1}"" /D:""{2}"" /P:""{3}"" /l:/e", scriptName, verdorId, deviceId, port);
            startinfo.Arguments = arguments;
            _logger.Info(string.Format("File patch: {0}, file name: {1}", filePatch, arguments));

            //Assign Procesing status
            Process.Start(startinfo);
            //wait for 3 s to DM process start
            Thread.Sleep(25000);

            _logger.Info(string.Format(string.Format("Done call DM Master app", DateTime.UtcNow)));

            Process[] processlist = Process.GetProcesses();

            foreach (Process theprocess in processlist)
            {
                //_logger.Info(string.Format(string.Format("Processes: {0}, {1}", theprocess.ProcessName, theprocess.MainWindowTitle)));
                if (theprocess.MainWindowTitle == ALL_NETWORK_ARE_BUSY_TITLE)
                {
                    _logger.Info(string.Format(string.Format("All net work liense DM are busy {0}", DateTime.UtcNow)));
                    theprocess.Kill();
                    result = 1;
                }
                if (theprocess.MainWindowTitle == ALL_NETWORK_ARE_FAIL_TITLE)
                {
                    _logger.Info(string.Format(string.Format("Failed to connect VPN {0}", DateTime.UtcNow)));
                    theprocess.Kill();
                    result = 2;
                }
            }
        }

    }
}

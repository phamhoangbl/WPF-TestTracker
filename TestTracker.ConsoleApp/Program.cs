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

namespace TestTracker.ConsoleApp
{
    public class Program
    {
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
            textDebug += String.Format("Port: {0}", options.Port) + "\n";
            textDebug += String.Format("Other Options: {0}", options.OtherOption) + " \n";

            _logger.Info(textDebug);

            try
            {
                CallDmMaster(testQueueId, filePatch, scriptName, verdorId, deviceId, port, otherOption);
                //Update status when have done run DM Master
                var testQueueRepository = new TestQueueRepository();
                testQueueRepository.UpdateStatus(int.Parse(testQueueId), EnumTestStatus.Completed);
                textDebug += "Done";
                textDebug += "---------------------------------------";
                _logger.Info(textDebug);
            }
            catch(Exception ex)
            {
                _logger.Info(string.Format("Error when trying to update status: {0}", ex.Message));
            }

        }

        private static void CallDmMaster(string testQueueId, string filePatch, string scriptName, string verdorId, string deviceId, string port, string orderOption)
        {

            // Run Multi script Setup the process with the ProcessStartInfo class.
            // Retrieve Queues have status = Running

            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.UseShellExecute = false;
            startinfo.CreateNoWindow = true;
            startinfo.WindowStyle = ProcessWindowStyle.Hidden;
            startinfo.FileName = filePatch;
            startinfo.RedirectStandardOutput = true;

            startinfo.Arguments = string.Format(@"/s:""{0}"" /v:""{1}"" /D:""{2}"" /P:""{3}"" /l:/e", scriptName, verdorId, deviceId, port);

            //Assign Procesing status
            using (Process process = Process.Start(startinfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    _logger.Info(result);
                }
            }
        }

    }
}

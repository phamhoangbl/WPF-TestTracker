using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace TestTracker.ConsoleApp
{
    class Options
    {
        [Option('i', "testQueueId", Required = true, HelpText = "Input Test Queue Id to process.")]
        public string TestQueueId { get; set; }

        [Option('f', "filePath", Required = true,
        HelpText = "Input file path DM master.exe to be processed.")]
        public string FilePath { get; set; }

        [Option('s', "scriptName", Required = true, HelpText = "Input script name to process.")]
        public string ScriptName { get; set; }

        [Option('v', "verdorId", Required = true, HelpText = "Input Verdor ID to process.")]
        public string VerdorId { get; set; }

        [Option('d', "deviceId", Required = true, HelpText = "Input Device ID to process.")]
        public string DeviceId { get; set; }

        [Option('p', "port", Required = true, HelpText = "Input Port to process.")]
        public string Port { get; set; }

        [Option('o', "otherOption", Required = false, HelpText = "Input Port to process.")]
        public string OtherOption { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
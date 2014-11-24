using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTracker.Core.Utils
{
    public class FileResult
    {
        public string FileName { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string DeviceName { get; set; }
        public string HBAName { get; set; }
        public string TotalLBA { get; set; }
        public string Capacity { get; set; }
        public string ScriptStartTime { get; set; }
        public string ScriptEndTime { get; set; }
        public string ScriptEndDate{ get; set; }
        public string TotalRuntime { get; set; }
        public string Time { get; set; }
        public string TotalOfErrors { get; set; }
        public string TotalOfCommands { get; set; }
        public string ModelNumber { get; set; }
        public string ScriptName { get; set; }
        public string SerialNumber { get; set; }
        public string FWRevision { get; set; }
        public string TestedBy { get; set; }
    }
}

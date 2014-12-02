using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace TestTracker.Core.Data.Model
{
    [Table("TestResult")]
    public class TestResult
    {
        [Key]
        public int TestResultId { get; set; }
        public int TestQueueId { get; set; }
        public string FileName { get; set; }
        public string DeviceName { get; set; }
        public string HBAName { get; set; }
        public string TotalLBA { get; set; }
        public string Capacity { get; set; }
        public string ScriptStartTime { get; set; }
        public string ScriptStartDate { get; set; }
        public string ScriptEndTime { get; set; }
        public string ScriptEndDate{ get; set; }
        public string TotalRuntime { get; set; }
        public string TotalOfErrors { get; set; }
        public string TotalOfCommands { get; set; }
        public string ModelNumber { get; set; }
        public string ScriptName { get; set; }
        public string SerialNumber { get; set; }
        public string FWRevision { get; set; }
        public string TestedBy { get; set; }
        public DateTime CreatedDateUtc { get; set; }
    }
}

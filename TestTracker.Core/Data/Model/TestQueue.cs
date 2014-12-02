using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TestTracker.Core.Data.Model
{
    [Table("TestQueue")]
    public class TestQueue
    {
        [Key]
        public int TestQueueId { get; set; }
        public string ScriptName { get; set; }
        public int TestStatusId { get; set; }
        public int TestStuffId { get; set; }
        public int? TestResultId { get; set; }
        public DateTime StartedTime { get; set; }
        public DateTime? FinishedTime { get; set; }
    }
}

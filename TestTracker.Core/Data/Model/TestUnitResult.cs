using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TestTracker.Core.Data.Model
{
    [Table("TestUnitResult")]
    public class TestUnitResult
    {
        public int TestUnitResultId { get; set; }
        public int TestQueueId { get; set; }
        public string TestUnitName { get; set; }
        public string TestValue { get; set; }
        public DateTime CreatedDateUtc { get; set; }
    }
}

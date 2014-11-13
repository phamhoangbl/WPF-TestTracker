using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TestTracker.Core.Data.Model
{
    [Table("TestStuff")]
    public class TestStuff
    {
        public int TestStuffId { get; set; }
        public string DeviceId { get; set; }
        public string VerdorId { get; set; }
        public string Port { get; set; }
        public string ComputerName { get; set; }
    }
}

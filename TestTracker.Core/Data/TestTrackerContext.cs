using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTracker.Core.Data.Model
{
    public class TestTrackerContext : DbContext
    {
        public DbSet<TestQueue> TestQueues { get; set; }
        public DbSet<TestStuff> TestStuffs { get; set; }
    }
}

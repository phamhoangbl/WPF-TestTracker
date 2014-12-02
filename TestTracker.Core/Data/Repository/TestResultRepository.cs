using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTracker.Core.Data.Model;
using TestTracker.Core.Data.Repository;
using TestTracker.Core.Utils;

namespace TestTracker.Core.Data.Repository
{
    public class TestResultRepository : ITestResultRepository
    {
        private TestTrackerContext db = null;

        public TestResultRepository()
        {
            this.db = new TestTrackerContext();
        }

        public TestResultRepository(TestTrackerContext db)
        {
            this.db = db;
        }

        public TestResult RetrieveTestResult(int id)
        {
            return db.TestResults.Find(id);
        }

        public List<TestResult> RetrieveTestResultByTestQueueId(int testQueueId)
        {
            return db.TestResults.Where(x => x.TestQueueId == testQueueId).OrderBy(x => x.TestQueueId).ToList();
        }

        public void InsertTestResult(TestResult obj)
        {
            db.TestResults.Add(obj);
            db.SaveChanges();
        }
    }
}

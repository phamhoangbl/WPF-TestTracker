using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTracker.Core.Data.Model;

namespace TestTracker.Core.Data.Repository
{
    public class TestResultDocumentRepository : ITestResultDocumentRepository
    { 
        private TestTrackerContext db = null;

        public TestResultDocumentRepository()
        {
            this.db = new TestTrackerContext();
        }

        public TestResultDocumentRepository(TestTrackerContext db)
        {
            this.db = db;
        }
        public List<TestResultDocument> RetrieveTestResultDocumentByTestQueueId(int testQueueId)
        {
            return db.TestResultDocuments.Where(x => x.TestQueueId == testQueueId).OrderBy(x => x.TestQueueId).ToList();
        }
        public void InsertTestResultDocument(TestResultDocument obj)
        {
            db.TestResultDocuments.Add(obj);
            db.SaveChanges();
        }
    }
}

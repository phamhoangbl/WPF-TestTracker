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
    public class TestUnitResultRepository : ITestUnitResultRepository
    {
        private TestTrackerContext db = null;

        public TestUnitResultRepository()
        {
            this.db = new TestTrackerContext();
        }

        public TestUnitResultRepository(TestTrackerContext db)
        {
            this.db = db;
        }

        public TestUnitResult RetrieveTestUnitResult(int id)
        {
            return db.TestUnitResults.Find(id);
        }

        public void InsertTestUnitResult(TestUnitResult obj)
        {
            db.TestUnitResults.Add(obj);
            db.SaveChanges();
        }
    }
}

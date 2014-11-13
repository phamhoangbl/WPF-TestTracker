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
    public class TestQueueRepository : ITestQueueRepository
    {
        private TestTrackerContext db = null;

         public TestQueueRepository()
         {
             this.db = new TestTrackerContext();
         }

         public TestQueueRepository(TestTrackerContext db)
         {
             this.db = db;
         }

         public IEnumerable<TestQueue> SelectAll()
         {
             //get queues of client computer 
             var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
             return db.TestQueues.ToList().Where(x => testStuffId.Contains(x.TestStuffId)).OrderBy(x => x.TestQueueId);
         }

         public TestQueue SelectQueueRunning()
         {
             //get queues of client computer 
             var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
             var testQueue = db.TestQueues.ToList().Where(x => testStuffId.Contains(x.TestStuffId)).OrderBy(x => x.TestQueueId);
             return testQueue.Where(x => x.TestStatusId == 5).SingleOrDefault();
         }

         public TestQueue SelectByID(string id)
         {
             return db.TestQueues.Find(id);
         }

         public void Insert(TestQueue obj)
         {
             db.TestQueues.Add(obj);
             db.SaveChanges();
         }

         public void Update(TestQueue obj)
         {
             db.Entry(obj).State = EntityState.Modified;
         }

         public void Delete(string id)
         {
             TestQueue existing = db.TestQueues.Find(id);
             db.TestQueues.Remove(existing);
         }

         public void Save()
         {
             db.SaveChanges();
         }

         public bool HasRunning()
         {
             var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
             bool hasRunning = db.TestQueues.Any(x => x.TestStatusId == (int)EnumTestStatus.Running && testStuffId.Contains(x.TestStuffId));
             return hasRunning;
         }
    }
}

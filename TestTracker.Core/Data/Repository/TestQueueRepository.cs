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
             var running = testQueue.Where(x => x.TestStatusId ==(int)EnumTestStatus.Running).FirstOrDefault();
             return running;
         }
         public TestQueue SelectQueueStopped()
         {
             //get queues of client computer 
             var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
             var testQueue = db.TestQueues.ToList().Where(x => testStuffId.Contains(x.TestStuffId)).OrderBy(x => x.TestQueueId);
             var running = testQueue.Where(x => x.TestStatusId == (int)EnumTestStatus.Stopped).FirstOrDefault();
             return running;
         }
         public TestQueue SelectQueueProcessing()
         {
             //get queues of client computer 
             var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
             var testQueue = db.TestQueues.ToList().Where(x => testStuffId.Contains(x.TestStuffId)).OrderBy(x => x.TestQueueId);
             return testQueue.Where(x => x.TestStatusId == 6).SingleOrDefault();
         }

         public void UpdateStatus(int testQueueId, EnumTestStatus newStatus)
         {
             var testQueue = db.TestQueues.Find(testQueueId);
             testQueue.TestStatusId = (int)newStatus;
             Update(testQueue);

             //When done update new status for testQueue = Completed, Umcompleted, assign new Running Status for next test queue
             if (newStatus == EnumTestStatus.Completed || newStatus == EnumTestStatus.Uncompleted)
             {
                 //update finished time
                 testQueue.FinishedTime = DateTime.UtcNow;

                 var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
                 var testQueueNext = db.TestQueues.OrderBy(x => x.TestQueueId).FirstOrDefault(x => testStuffId.Contains(x.TestStuffId) && (x.TestStatusId == (int)EnumTestStatus.Pending
                                                                                                        || x.TestStatusId == (int)EnumTestStatus.Uncompleted));
                 if (testQueueNext != null)
                 {
                     testQueueNext.TestStatusId = (int)EnumTestStatus.Running;
                     Update(testQueueNext);
                 }
             }

             db.SaveChanges();
         }
            
         public TestQueue SelectByID(int id)
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

         public void UpdateAndSave(TestQueue obj)
         {
             db.Entry(obj).State = EntityState.Modified;
             db.SaveChanges();
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
             return db.TestQueues.Any(x => x.TestStatusId == (int)EnumTestStatus.Running && testStuffId.Contains(x.TestStuffId));
         }

         public TestQueue SelectTestQueueProcessing()
         {
             //get queues of client computer 
             var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
             return db.TestQueues.FirstOrDefault(x => x.TestStatusId == (int)EnumTestStatus.Processing && testStuffId.Contains(x.TestStuffId));
         }

         public TestQueue MakeQueueRunning()
         {
             var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
             var testQueueNext = db.TestQueues.OrderBy(x => x.TestQueueId).FirstOrDefault(x => testStuffId.Contains(x.TestStuffId) && (x.TestStatusId == (int)EnumTestStatus.Pending
                                                                                                    || x.TestStatusId == (int)EnumTestStatus.Uncompleted));
             if (testQueueNext != null)
             {
                 testQueueNext.TestStatusId = (int)EnumTestStatus.Running;
                 UpdateAndSave(testQueueNext);
             }

             return testQueueNext;
         }

         public TestQueue MakeQueueRunning(int testQueueProcessingId)
         {
             TestQueue processingQueue = SelectByID(testQueueProcessingId);
             processingQueue.TestStatusId = (int)EnumTestStatus.Running;
             UpdateAndSave(processingQueue);

             return processingQueue;
         }
    }
}

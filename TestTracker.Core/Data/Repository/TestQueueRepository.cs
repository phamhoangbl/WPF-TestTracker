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

         public IEnumerable<TestQueue> SelectAllTestQueue()
         {
             //get queues of client computer 
             var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
             return db.TestQueues.ToList().Where(x => testStuffId.Contains(x.TestStuffId)).OrderBy(x => x.TestQueueId);
         }

         public TestQueue SelectQueueStopped()
         {
             //get queues of client computer 
             var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
             var testQueue = db.TestQueues.ToList().Where(x => testStuffId.Contains(x.TestStuffId)).OrderBy(x => x.TestQueueId);
             var running = testQueue.Where(x => x.TestStatusId == (int)EnumTestStatus.Stopped).FirstOrDefault();
             return running;
         }

         public void UpdateTestQueueStatus(int testQueueId, EnumTestStatus newStatus)
         {
             var testQueue = db.TestQueues.Find(testQueueId);
             testQueue.TestStatusId = (int)newStatus;
             testQueue.FinishedTime = DateTime.UtcNow;
             UpdateAndSaveTestQueue(testQueue);

             //assign new Running Status for next test queue
             if (newStatus != EnumTestStatus.Processing)
             {
                 var testQueueNext = RetrieveTestQueueNotCompleted(testQueueId);
                 if (testQueueNext != null)
                 {
                     testQueueNext.TestStatusId = (int)EnumTestStatus.Running;
                     UpdateAndSaveTestQueue(testQueueNext);
                 }
             }
         }

         public void SaveTestQueue()
         {
             db.SaveChanges();
         }

         public TestQueue RetrieveTestQueueNotCompleted()
         {
             //priority get: 1.Running, 2.Processing, 3.FailConnection, 4.BusyConnection, 5.WrongPort, 6.Stopped, 7.Uncompleted
             var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
             if (db.TestQueues.Any(x => x.TestStatusId == (int)EnumTestStatus.Running && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestStatusId == (int)EnumTestStatus.Running && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestStatusId == (int)EnumTestStatus.Processing && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestStatusId == (int)EnumTestStatus.Processing && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestStatusId == (int)EnumTestStatus.FailConnection && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestStatusId == (int)EnumTestStatus.FailConnection && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestStatusId == (int)EnumTestStatus.BusyConnection && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestStatusId == (int)EnumTestStatus.BusyConnection && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestStatusId == (int)EnumTestStatus.WrongHBAConfig && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestStatusId == (int)EnumTestStatus.WrongHBAConfig && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestStatusId == (int)EnumTestStatus.Stopped && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestStatusId == (int)EnumTestStatus.Stopped && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestStatusId == (int)EnumTestStatus.Uncompleted && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestStatusId == (int)EnumTestStatus.Uncompleted && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestStatusId == (int)EnumTestStatus.Pending && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestStatusId == (int)EnumTestStatus.Pending && testStuffId.Contains(x.TestStuffId));
             }
             return null;
         }

         public TestQueue RetrieveTestQueueNotCompleted(int nextTestQueueId)
         {
             //priority get: 1.Running, 2.Processing, 3.FailConnection, 4.BusyConnection, 5.WrongPort, 6.Stopped, 7.Uncompleted
             var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
             if (db.TestQueues.Any(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.Running && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.Running && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.Processing && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.Processing && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.FailConnection && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.FailConnection && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.BusyConnection && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.BusyConnection && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.WrongHBAConfig && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.WrongHBAConfig && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.Stopped && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.Stopped && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.Uncompleted && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.Uncompleted && testStuffId.Contains(x.TestStuffId));
             }
             if (db.TestQueues.Any(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.Pending && testStuffId.Contains(x.TestStuffId)))
             {
                 return db.TestQueues.FirstOrDefault(x => x.TestQueueId != nextTestQueueId && x.TestStatusId == (int)EnumTestStatus.Pending && testStuffId.Contains(x.TestStuffId));
             }
             return null;
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

         public bool IsTestQueueRunning(int testQueueId)
         {
             var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
             return db.TestQueues.Any(x => x.TestQueueId == testQueueId && x.TestStatusId == (int)EnumTestStatus.Running && testStuffId.Contains(x.TestStuffId));
         }
         //public TestQueue MakeQueueRunning()
         //{
         //    var testStuffId = db.TestStuffs.Where(x => x.ComputerName == System.Environment.MachineName).Select(x => x.TestStuffId);
         //    var testQueueNext = db.TestQueues.OrderBy(x => x.TestQueueId).FirstOrDefault(x => testStuffId.Contains(x.TestStuffId) && (x.TestStatusId == (int)EnumTestStatus.Pending
         //                                                                                           || x.TestStatusId == (int)EnumTestStatus.Uncompleted));
         //    if (testQueueNext != null)
         //    {
         //        testQueueNext.TestStatusId = (int)EnumTestStatus.Running;
         //        UpdateAndSaveTestQueue(testQueueNext);
         //    }

         //    return testQueueNext;
         //}

         //public TestQueue MakeQueueRunning(int testQueueProcessingId)
         //{
         //    TestQueue processingQueue = SelectTestQueueByID(testQueueProcessingId);
         //    processingQueue.TestStatusId = (int)EnumTestStatus.Running;
         //    UpdateAndSaveTestQueue(processingQueue);

         //    return processingQueue;
         //}

         public TestQueue RetrieveTestQueue(int id)
         {
             return db.TestQueues.Find(id);
         }

         public void InsertTestQueue(TestQueue obj)
         {
             db.TestQueues.Add(obj);
             db.SaveChanges();
         }

         public void UpdateTestQueue(TestQueue obj)
         {
             db.Entry(obj).State = EntityState.Modified;
         }

         public void UpdateAndSaveTestQueue(TestQueue obj)
         {
             db.Entry(obj).State = EntityState.Modified;
             db.SaveChanges();
         }

         public void DeleteTestQueue(string id)
         {
             TestQueue existing = db.TestQueues.Find(id);
             db.TestQueues.Remove(existing);
         }
    }
}

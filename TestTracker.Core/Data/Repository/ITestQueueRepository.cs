using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTracker.Core.Data.Model;
using TestTracker.Core.Utils;

namespace TestTracker.Core.Data.Repository
{
    public interface ITestQueueRepository
    {
        IEnumerable<TestQueue> SelectAllTestQueue();
        TestQueue RetrieveTestQueue(int id);
        void InsertTestQueue(TestQueue obj);
        void UpdateTestQueueStatus(int testQueueId, EnumTestStatus newStatus);
        void UpdateTestQueue(TestQueue obj);
        void DeleteTestQueue(string id);
        TestQueue RetrieveTestQueueNotCompleted();
        TestQueue SelectTestQueueProcessing();
        void SaveTestQueue();
    }
}

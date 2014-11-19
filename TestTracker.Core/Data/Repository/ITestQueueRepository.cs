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
        IEnumerable<TestQueue> SelectAll();
        TestQueue SelectByID(int id);
        void Insert(TestQueue obj);
        void UpdateStatus(int testQueueId, EnumTestStatus newStatus);
        void Update(TestQueue obj);
        void Delete(string id);
        bool HasRunning();
        void Save();
    }
}

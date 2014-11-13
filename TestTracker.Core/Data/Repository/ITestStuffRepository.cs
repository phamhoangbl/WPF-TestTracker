using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTracker.Core.Data.Model;

namespace TestTracker.Core.Data.Repository
{
    public interface ITestStuffRepository
    {
        IEnumerable<TestStuff> SelectAll();
        TestStuff SelectByID(int id);
        TestStuff Select(string deviceId, string verdorId, string port);
        void Insert(TestStuff obj, out int testStuffId);
        void Update(TestStuff obj);
        void Delete(string id);
        void Save();
    }
}

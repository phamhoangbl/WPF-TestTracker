using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTracker.Core.Data.Model;
using TestTracker.Core.Utils;

namespace TestTracker.Core.Data.Repository
{
    public interface ITestResultRepository
    {
        TestResult RetrieveTestResult(int id);
        void InsertTestResult(TestResult obj);
    }
}

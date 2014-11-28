using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTracker.Core.Data.Model;
using TestTracker.Core.Utils;

namespace TestTracker.Core.Data.Repository
{
    public interface ITestUnitResultRepository
    {
        TestUnitResult RetrieveTestUnitResult(int id);
        void InsertTestUnitResult(TestUnitResult obj);
    }
}

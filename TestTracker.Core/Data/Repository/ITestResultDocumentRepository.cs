using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTracker.Core.Data.Model;

namespace TestTracker.Core.Data.Repository
{
    public interface ITestResultDocumentRepository
    {
        List<TestResultDocument> RetrieveTestResultDocumentByTestQueueId(int id);
        void InsertTestResultDocument(TestResultDocument obj);
    }
}

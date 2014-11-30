using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using TestTracker.Core.Data.Model;
using TestTracker.Core.Data.Repository;

namespace TestTracker.Controls.Grid
{
    public class TestQueueViewModel
    {
        public ICollectionView TestQueues { get; private set; }


        public TestQueueViewModel()
        {
            var testQueueRepository = new TestQueueRepository();
            var _testQueue = testQueueRepository.SelectAllTestQueue();
            
            TestQueues = CollectionViewSource.GetDefaultView(_testQueue);

        }
    }
}

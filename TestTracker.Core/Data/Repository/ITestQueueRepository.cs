﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTracker.Core.Data.Model;

namespace TestTracker.Core.Data.Repository
{
    public interface ITestQueueRepository
    {
        IEnumerable<TestQueue> SelectAll();
        TestQueue SelectByID(string id);
        void Insert(TestQueue obj);
        void Update(TestQueue obj);
        void Delete(string id);
        bool HasRunning();
        void Save();
    }
}
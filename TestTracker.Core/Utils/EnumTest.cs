﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTracker.Core.Utils
{
    public enum EnumTestStatus
    {
        Pending = 1,
        Completed = 2,
        Uncompleted = 3,
        Stopped = 4,
        Running = 5,
        Processing = 6,
        FailConnection = 7,
        BusyConnection = 8,
        WrongHBAConfig = 9,
        WrongPort = 10
    };
}

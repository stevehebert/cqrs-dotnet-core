﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cqrs.Messaging.Handling
{
    public interface IProcessor
    {
        void Start();
        void Stop();
    }
}

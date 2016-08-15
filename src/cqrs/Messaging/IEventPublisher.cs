﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cqrs.Messaging
{
    public interface IEventPublisher
    {
        IEnumerable<IEvent> Events { get; }
    }
}
using System;
using System.Collections.Generic;

namespace cqrs.Messaging
{
    public interface ICommand
    {
        Guid Id { get; }
    }

   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cqrs_test.Database
{
    public interface IAggregateRoot
    {
        Guid Id { get; }
    }
}

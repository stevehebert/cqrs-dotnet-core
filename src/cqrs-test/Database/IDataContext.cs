using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cqrs_test.Database
{
    public interface IDataContext<T> : IDisposable
        where T : IAggregateRoot
    {
        T Find(Guid id);

        void Save(T aggregate);
    }
}

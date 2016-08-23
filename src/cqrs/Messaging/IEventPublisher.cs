using System.Collections.Generic;

namespace cqrs.Messaging
{
    public interface IEventPublisher
    {
        IEnumerable<IEvent> Events { get; }
    }
}

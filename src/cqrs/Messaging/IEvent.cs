using System;

namespace cqrs.Messaging
{
    public interface IEvent
    {
        Guid SourceId { get; }
    }
}

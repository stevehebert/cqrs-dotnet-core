using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cqrs.Messaging
{
    public interface IEventBus
    {
        void Publish(Envelope<IEvent> @event);
        void Publish(IEnumerable<Envelope<IEvent>> events);
    }
}

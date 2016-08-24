using System;
using System.Collections.Generic;
using cqrs.Serialization;

namespace cqrs.Messaging
{
    public abstract class EventBus : IEventBus, IDisposable
    {
        private IMessageSender _sender;
        protected IMetadataProvider MetadataProvider { get; private set; }
        protected ITextSerializer TextSerializer { get; private set; }

        protected EventBus(IMessageSender sender, IMetadataProvider metadataProvider, ITextSerializer textSerializer)
        {
            _sender = sender;
            MetadataProvider = metadataProvider;
            TextSerializer = textSerializer;
        }

        public void Publish(Envelope<IEvent> @event)
        {
            _sender.Send(BuildMessage(@event));
        }

        public void Publish(IEnumerable<Envelope<IEvent>> events)
        {
            foreach (var @event in events)
            {
                this.Publish(@event);
            }
        }

        public void Dispose()
        {
            _sender?.Dispose();
            _sender = null;
        }

        protected abstract IBrokeredMessage BuildMessage(Envelope<IEvent> envelope);

    }
}
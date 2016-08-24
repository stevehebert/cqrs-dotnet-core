using System;
using System.IO;
using System.Text;
using cqrs.Messaging;
using cqrs.Serialization;
using NetMQ;

namespace cqrs.ZeroMQ.Messaging
{
    public class ZeroMQEventBus : EventBus
    {
        public ZeroMQEventBus(IMessageSender sender, IMetadataProvider metadataProvider, ITextSerializer textSerializer) : base(sender, metadataProvider, textSerializer)
        {
        }

        public ZeroMQEventBus(IZeroMQConfig config, IMetadataProvider metadataProvider, ITextSerializer textSerializer)
            : base(new ZeroMQMessageSender(config), metadataProvider, textSerializer)
        { }

        protected override IBrokeredMessage BuildMessage(Envelope<IEvent> envelope)
        {
            var message = new Msg();
            using (var bodyWriter = new StringWriter())
            using (var writer = new StringWriter())
            {
                TextSerializer.Serialize(bodyWriter, envelope.Body);
                var messageContent = new ZeroMQCommandBus.ZeroMQMessageContent
                {
                    Data = bodyWriter.ToString(),
                    Properties = this.MetadataProvider.GetMetadata(envelope.Body)
                };

                messageContent.Properties.Add("id", envelope.MessageId);
                messageContent.Properties.Add("cor-id", envelope.CorrelationId);

                this.TextSerializer.Serialize(writer, messageContent);
                var bytArray = Encoding.UTF8.GetBytes(writer.ToString());
                message.InitGC(bytArray, bytArray.Length);
            }

            return new ZeroMqMessage
            {
                Message = message,
                Payload = envelope.Body,
                MessageId = envelope.Body.SourceId.ToString(),
                CorrelationId = string.IsNullOrEmpty(envelope.CorrelationId) ? Guid.NewGuid().ToString() : envelope.CorrelationId
            };
        }
    }
}
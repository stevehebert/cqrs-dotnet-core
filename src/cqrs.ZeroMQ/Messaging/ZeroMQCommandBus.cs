using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using cqrs.Messaging;
using cqrs.Serialization;
using NetMQ;

namespace cqrs.ZeroMQ.Messaging
{
    public class ZeroMqMessage : IBrokeredMessage
    {
        public Msg Message {get;set;}
        public string MessageId { get; set; }
        public string CorrelationId { get; set; }
        public string TraceId { get; set; }
        public object Payload { get; set; }
        public int DeliveryCount { get; set; }
    }



    public class ZeroMQCommandBus : CommandBus
    {
        public class ZeroMQMessageContent
        {
            public string Data { get; set; }
            public IDictionary<String, String> Properties { get; set; }
        }
        public ZeroMQCommandBus(IMessageSender sender, IMetadataProvider metadataProvider, ITextSerializer serializer) : base(sender, metadataProvider, serializer)
        { }

        public ZeroMQCommandBus(IZeroMQConfig config, IMetadataProvider metadataProvider, ITextSerializer serializer) : base(new ZeroMQMessageSender(config), metadataProvider, serializer)
        { }

        public override IBrokeredMessage BuildMessage<TCommand>(Envelope<TCommand> command)
        {
            var message = new Msg();
            using (var bodyWriter = new StringWriter())
            using (var writer = new StringWriter())
            {
                this.serializer.Serialize(bodyWriter, command.Body);
                var messageContent = new ZeroMQMessageContent
                {
                    Data = bodyWriter.ToString(),
                    Properties = this.metadataProvider.GetMetadata(command.Body)
                };

                messageContent.Properties.Add("id", command.MessageId);
                messageContent.Properties.Add("cor-id", command.CorrelationId);
                
                this.serializer.Serialize(writer, messageContent);
                var bytArray = Encoding.UTF8.GetBytes(writer.ToString());
                message.InitGC(bytArray, bytArray.Length);
            }

            return new ZeroMqMessage
            {
                Message = message,
                Payload = command.Body,
                MessageId = command.Body.Id.ToString(),
                CorrelationId = string.IsNullOrEmpty(command.CorrelationId) ? Guid.NewGuid().ToString() : command.CorrelationId
            };
        }
    }
}

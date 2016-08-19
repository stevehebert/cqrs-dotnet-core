using System;
using cqrs.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NetMQ;

namespace cqrs.Messaging
{
    public class ZeroMqMessage : IBrokeredMessage
    {
        public NetMQMessage Message {get;set;}
    }



    public class ZeroMQCommandBus : CommandBus
    {
        public class ZeroMQMessageContent
        {
            public string Data { get; set; }
            public IDictionary<String, String> Properties { get; set; }
        }
        public ZeroMQCommandBus(IMessageSender sender, IMetadataProvider metadataProvider, ITextSerializer serializer) : base(sender, metadataProvider, serializer)
        {
        }

        public override IBrokeredMessage BuildMessage<TCommand>(Envelope<TCommand> command)
        {
            var message = new NetMQMessage(1);
            using (var bodyWriter = new StringWriter())
            using (var writer = new StringWriter())
            {
                this.serializer.Serialize(bodyWriter, command.Body);
                var messageContent = new ZeroMQMessageContent
                {
                    Data = bodyWriter.ToString(),
                    Properties = this.metadataProvider.GetMetadata(command.Body)
                };

                this.serializer.Serialize(writer, messageContent);
                var bytArray = ASCIIEncoding.UTF8.GetBytes(writer.ToString());

                message.Append(bytArray);
            }

            return new ZeroMqMessage
            {
                Message = message
            };
        }
    }
}

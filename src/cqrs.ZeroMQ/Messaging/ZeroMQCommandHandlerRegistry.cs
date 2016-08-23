using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using cqrs.Messaging;
using cqrs.Messaging.Handling;
using NetMQ;
using NetMQ.Sockets;

namespace cqrs.ZeroMQ.Messaging
{
    public class ZeroMQMessageReceiver : IMessageReceiver<Msg>
    {
        private readonly ResponseSocket _responseSocket;

        public ZeroMQMessageReceiver(IZeroMQConfig config)
        {
            _responseSocket = new ResponseSocket(config.ResponseSocket);
        }
        public void Start(Func<Msg, MessageReleaseAction> messageHandler)
        {
            Msg msg = new Msg();
            _responseSocket.ReceiveReady += (sender, args) =>
            {
                args.Socket.Receive(ref msg);
 
                messageHandler(msg);
            };
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }

    public class ZeroMQCommandHandlerRegistry : CommandHandlerRegistry<Msg>
    {
        public ZeroMQCommandHandlerRegistry(IMessageReceiver<Msg> messageReceiver, ITextSerializer serializer, IDeliveryConfiguration deliveryConfiguration) : base(messageReceiver, serializer, deliveryConfiguration)
        {
        }

        protected override IBrokeredMessage TranslateIncomingMessage(Msg message)
        {
            var content = System.Text.Encoding.UTF8.GetString(message.Data);

            var bareObject = this.Serializer.Deserialize(new StringReader(content)) as ZeroMQCommandBus.ZeroMQMessageContent;

           
            var body = this.Serializer.Deserialize(new StringReader(bareObject.Data));
            return new ZeroMqMessage()
            {
                Message = message,
                DeliveryCount = 0,
                Payload = body,
                MessageId = bareObject.Properties["id"],
                CorrelationId = bareObject.Properties["cor-id"]
            };
        }
    }
}

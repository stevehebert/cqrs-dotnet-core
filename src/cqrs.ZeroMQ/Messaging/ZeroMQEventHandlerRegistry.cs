using System.IO;
using cqrs.Messaging;
using cqrs.Messaging.Handling;
using NetMQ;

namespace cqrs.ZeroMQ.Messaging
{
    public class ZeroMQEventHandlerRegistry : CommandHandlerRegistry<Msg>
    {
        public ZeroMQEventHandlerRegistry(IMessageReceiver<Msg> messageReceiver, 
            ITextSerializer serializer, 
            IDeliveryConfiguration deliveryConfiguration) : base(messageReceiver, serializer, deliveryConfiguration)
        {}

        public ZeroMQEventHandlerRegistry(IZeroMQConfig config,
            ITextSerializer serializer,
            IDeliveryConfiguration deliveryConfiguration)
            : base(new ZeroMQMessageReceiver(config), serializer, deliveryConfiguration)
        {}


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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cqrs.ZeroMQ.Messaging;
using NetMQ;
using NetMQ.Sockets;

namespace cqrs.Messaging
{
    public interface IZeroMQConfig
    {
        string RequestSocket { get; }
        string ResponseSocket { get; }
    }
    public class ZeroMQMessageSender : IMessageSender
    {
        private RequestSocket _requestSocket;

        public ZeroMQMessageSender(IZeroMQConfig config)
        {
            _requestSocket = new RequestSocket(config.RequestSocket);
        }
       

        public void Send(IBrokeredMessage message)
        {
            var zero = message as ZeroMqMessage;
            var msg = zero.Message;

            _requestSocket.TrySend(ref msg, TimeSpan.Zero, false);
        }

        public void Send(IEnumerable<IBrokeredMessage> messages)
        {
            foreach (var message in messages)
            {
                this.Send(message);
            }
        }
    }
}

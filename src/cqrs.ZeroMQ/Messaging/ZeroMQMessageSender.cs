using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cqrs.Messaging
{
    public class ZeroMQMessageSender : IMessageSender
    {
        public event EventHandler Retrying;

        public void Send(Func<IBrokeredMessage> messageFactory)
        {
            throw new NotImplementedException();
        }

        public void SendAsync(Func<IBrokeredMessage> messageFactory)
        {
            // throw new NotImplementedException();
        }

        public void SendAsync(Func<IBrokeredMessage> messageFactory, Action successCallback, Action<Exception> exceptionCallback)
        {
            throw new NotImplementedException();
        }
    }
}

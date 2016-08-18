using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cqrs.Messaging
{
    /// <summary>
    /// Abstracts the behavior of sending a message.
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Sends the specified message synchronously.
        /// </summary>
        void Send(Func<IBrokeredMessage> messageFactory);

        /// <summary>
        /// Sends the specified message asynchronously.
        /// </summary>
        void SendAsync(Func<IBrokeredMessage> messageFactory);

        /// <summary>
        /// Sends the specified message asynchronously.
        /// </summary>
        void SendAsync(Func<IBrokeredMessage> messageFactory, Action successCallback, Action<Exception> exceptionCallback);

        /// <summary>
        /// Notifies that the sender is retrying due to a transient fault.
        /// </summary>
        event EventHandler Retrying;
    }
}

using System;

namespace cqrs.Messaging
{
    public interface IMessageReceiver<T>
    {
        void Start(Func<T, MessageReleaseAction> messageHandler);

        void Stop();
    }
}

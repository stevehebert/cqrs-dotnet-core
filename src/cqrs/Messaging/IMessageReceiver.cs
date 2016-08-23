using System;

namespace cqrs.Messaging
{
    public interface IMessageReceiver<T> : IDisposable
    {
        void Start(Func<T, MessageReleaseAction> messageHandler);

        void Stop();
    }
}

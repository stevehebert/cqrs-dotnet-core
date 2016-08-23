using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Dynamic;



namespace cqrs.Messaging.Handling
{
    public interface IDeliveryConfiguration
    {
        int MaxProcessingRetries { get; }
    }

    public abstract class MessageProcessor<T> : IProcessor, IDisposable
    {
        private readonly IMessageReceiver<T> _messageReceiver;
        private readonly ITextSerializer _serializer;
        private readonly IDeliveryConfiguration _deliveryConfiguration;
        private readonly object _lockObject = new object();
        private bool _started = false;

        public MessageProcessor(IMessageReceiver<T> messageReceiver, ITextSerializer serializer, IDeliveryConfiguration deliveryConfiguration)
        {
            _messageReceiver = messageReceiver;
            _serializer = serializer;
            _deliveryConfiguration = deliveryConfiguration;
            if (!this._started)
            {
                this._messageReceiver.Start(this.OnMessageReceived);
                this._started = true;
            }
        }

        protected abstract IBrokeredMessage TranslateIncomingMessage(T message);
        protected abstract void ProcessMessage(IBrokeredMessage message);

        protected MessageReleaseAction OnMessageReceived(T message)
        {
            IBrokeredMessage brokeredMessage = null;
            try
            {
                brokeredMessage = TranslateIncomingMessage(message);
            }
            catch (Exception ex)
            {
                return MessageReleaseAction.DeadLetterMessage(ex.Message, ex.ToString());
            }

            try
            {
                ProcessMessage(brokeredMessage);
            }
            catch (Exception e)
            {
                return HandleProcessingException(brokeredMessage, e);
            }

            return CompleteMessage(brokeredMessage);
        }

        private MessageReleaseAction CompleteMessage(IBrokeredMessage message)
        {
            return MessageReleaseAction.CompleteMessage;
        }

        private MessageReleaseAction HandleProcessingException(IBrokeredMessage message, Exception e)
        {
            if (message.DeliveryCount > _deliveryConfiguration.MaxProcessingRetries)
            {
                Trace.TraceError("");
                return MessageReleaseAction.DeadLetterMessage(e.Message, e.ToString());
            }
            else
            {
                Trace.TraceWarning("");
                return MessageReleaseAction.AbandonMessage;
            }
        }

        protected ITextSerializer Serializer { get { return this._serializer; } }
        public void Start()
        {
            ThrowIfDisposed();

            lock (this._lockObject)
            {
                this._messageReceiver.Start(this.OnMessageReceived);
                this._started = true;
            }
        }

        public void Stop()
        {
            lock (this._lockObject)
            {
                if (this._started)
                {
                    this._messageReceiver.Stop();
                    this._started = false;
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _messageReceiver.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MessageProcessor() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        private void ThrowIfDisposed()
        {
            if (this.disposedValue)
                throw new ObjectDisposedException("MessageProcessor");
        }
    }
}

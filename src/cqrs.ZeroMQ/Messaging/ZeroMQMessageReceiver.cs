using System;
using System.Threading;
using cqrs.Messaging;
using NetMQ;
using NetMQ.Sockets;

namespace cqrs.ZeroMQ.Messaging
{
    public class ZeroMQMessageReceiver : IMessageReceiver<Msg>
    {
        private ResponseSocket _responseSocket;
        private INetMQPoller _poller;

        public ZeroMQMessageReceiver(IZeroMQConfig config)
        {
            _responseSocket = new ResponseSocket(config.ResponseSocket);
        }
        public void Start(Func<Msg, MessageReleaseAction> messageHandler)
        {
            _poller = new NetMQPoller();
            _responseSocket.ReceiveReady += (sender, args) =>
            {
                Msg msg = new Msg();
                msg.InitEmpty();
                args.Socket.Receive(ref msg);
 
                messageHandler(msg);
            };

            _poller.Add(_responseSocket);
            
            _poller.RunAsync();
        }

        public void Stop()
        {
            _responseSocket.Dispose();
            _responseSocket = null;
        }

        public void Dispose()
        {
            _poller?.Stop();
            _poller?.Dispose();
            _poller = null;
            _responseSocket?.Dispose();
            _responseSocket = null;
        }
    }
}
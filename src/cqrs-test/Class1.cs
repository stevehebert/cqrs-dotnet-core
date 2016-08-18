using cqrs.Messaging;
using NetMQ;
using NetMQ.Sockets;
using Xunit;

namespace cqrs_test
{
    public class Class1
    {
        [Fact]
        public void Go()
        {
            using (var server = new ResponseSocket("@tcp://localhost:5556")) // bind
            using (var client = new RequestSocket(">tcp://localhost:5556"))  // connect
            {
                // Send a message from the client socket
                client.SendFrame("Hello");

                // Receive the message from the server socket
                string m1 = server.ReceiveFrameString();
                Assert.Equal(m1, "Hello");


                // Send a response back from the server
                server.SendFrame("Hi Back");

                // Receive the response from the client socket
                string m2 = client.ReceiveFrameString();
                Assert.Equal(m2, "Hi Back");

            }
        }
    }
}

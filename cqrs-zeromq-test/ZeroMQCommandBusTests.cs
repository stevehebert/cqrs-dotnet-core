using cqrs.Messaging;
using cqrs.Serialization;
using Moq;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using cqrs.Messaging.Handling;
using cqrs.ZeroMQ.Messaging;
using Xunit;

namespace cqrs_zeromq_test
{
    public class ZeroMQCommandBusTests
    {
        public class NewItemCommand : ICommand
        {
            public NewItemCommand()
            {
                Id = Guid.NewGuid();
            }

            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public class NewItemCommandHandler : ICommandHandler<NewItemCommand>
        {
            public ManualResetEvent ResetEvent { get; private set; }

            public NewItemCommandHandler()
            {
                ResetEvent = new ManualResetEvent(false);
            }

            /*public void Handle(object command)
            {
                Trace.Write("NewItemCommand: " + command.Name);
                ResetEvent.Set();
            }*/
            public void Handle(NewItemCommand command)
            {
                Trace.Write("NewItemCommand: " + command.Name);
                ResetEvent.Set();
            }
        }

        [Fact]
        public void test_serialization()
        {
            var messageSenderMock = new Mock<IMessageSender>();
            var serializer = new JsonTextSerializer();

            var bus = new ZeroMQCommandBus(messageSenderMock.Object, new StandardMetadataProvider(), serializer );
            
            var message = bus.BuildMessage(Envelope.Create(new NewItemCommand() { Name = "Hello" })) as ZeroMqMessage;

            var content = System.Text.Encoding.UTF8.GetString(message.Message.Data);
            
            var bareObject = serializer.Deserialize<ZeroMQCommandBus.ZeroMQMessageContent>(content);

            var body = serializer.Deserialize<NewItemCommand>(bareObject.Data);
            bus.Dispose();

            Assert.Equal(body.Name, "Hello");
        }

        [Fact]
        public void register_handler_into_registry_works()
        {
            var mockConfig = new Mock<IZeroMQConfig>();
            var mockDeliveryConfig = new Mock<IDeliveryConfiguration>();
            mockDeliveryConfig.Setup(e => e.MaxProcessingRetries).Returns(10);
            mockConfig.Setup(m => m.RequestSocket).Returns(">tcp://localhost:5556");
            mockConfig.Setup(m => m.ResponseSocket).Returns("@tcp://localhost:5556");
            var serializer = new JsonTextSerializer();

            //var bus = new ZeroMQCommandBus(mockConfig.Object, new StandardMetadataProvider(), serializer);
            using (
                var receiver = new ZeroMQCommandHandlerRegistry(mockConfig.Object, serializer, mockDeliveryConfig.Object)
            )
            {
                var commandHandler = new NewItemCommandHandler();
                receiver.Register(commandHandler);

                Assert.Equal(1, receiver.RegisteredHandlerCount);
            }
        }

        [Fact]
        public void command_bus_sends_serialized_command()
        {
            var mockConfig = new Mock<IZeroMQConfig>();
            var mockDeliveryConfig = new Mock<IDeliveryConfiguration>();
            mockDeliveryConfig.Setup(e => e.MaxProcessingRetries).Returns(10);
            mockConfig.Setup(m => m.RequestSocket).Returns(">tcp://localhost:5556");
            mockConfig.Setup(m => m.ResponseSocket).Returns("@tcp://localhost:5556");
            var serializer = new JsonTextSerializer();

            using (var bus = new ZeroMQCommandBus(mockConfig.Object, new StandardMetadataProvider(), serializer))
            using (var receiver = new ZeroMQCommandHandlerRegistry(mockConfig.Object, serializer, mockDeliveryConfig.Object))
            {
                var commandHandler = new NewItemCommandHandler();
                receiver.Register(commandHandler);

                bus.Send(new Envelope<NewItemCommand>(new NewItemCommand() {Name = "test1"}));

                commandHandler.ResetEvent.WaitOne();
            }

        }
    }
}

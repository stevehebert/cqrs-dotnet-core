using cqrs.Messaging;
using cqrs.Serialization;
using Moq;
using System;
using System.IO;
using System.Text;
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

            Assert.Equal(body.Name, "Hello");
        }


        [Fact]
        public void command_bus_sends_serialized_command()
        {
            var mockConfig = new Mock<IZeroMQConfig>();
            mockConfig.Setup(m => m.RequestSocket).Returns(">tcp://localhost:5556");
            var serializer = new JsonTextSerializer();

            var bus = new ZeroMQCommandBus(mockConfig.Object, new StandardMetadataProvider(), serializer);

            bus.Send(new Envelope<NewItemCommand>(new NewItemCommand() {Name = "test1"}));




        }
    }
}

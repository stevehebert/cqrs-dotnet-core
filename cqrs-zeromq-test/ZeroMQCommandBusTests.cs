using cqrs.Messaging;
using cqrs.Serialization;
using Moq;
using System;
using System.IO;
using System.Text;
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


            var content = message.Message.First.ConvertToString(ASCIIEncoding.UTF8);
            var bareObject = serializer.Deserialize<ZeroMQCommandBus.ZeroMQMessageContent>( content);

            var body = serializer.Deserialize<NewItemCommand>(bareObject.Data);

            Assert.Equal(body.Name, "Hello");

            Assert.NotNull(body);
        }
    }
}

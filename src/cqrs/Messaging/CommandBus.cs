using cqrs.Serialization;
using System.Collections.Generic;

namespace cqrs.Messaging
{
    public abstract class CommandBus : ICommandBus
    {
        private readonly IMessageSender sender;
        private readonly IMetadataProvider metadataProvider;
        private readonly ITextSerializer serializer;

        public CommandBus(IMessageSender sender, IMetadataProvider metadataProvider, ITextSerializer serializer)
        {
            this.sender = sender;
            this.metadataProvider = metadataProvider;
            this.serializer = serializer;
        }
        /// <summary>
        /// Sends the specified command.
        /// </summary>
        public void Send(Envelope<ICommand> command)
        {
            this.sender.Send(() => BuildMessage(command));
        }

        public void Send(IEnumerable<Envelope<ICommand>> commands)
        {
            foreach (var command in commands)
            {
                this.Send(command);
            }
        }

        abstract public IBrokeredMessage BuildMessage(Envelope<ICommand> command);
    }
}

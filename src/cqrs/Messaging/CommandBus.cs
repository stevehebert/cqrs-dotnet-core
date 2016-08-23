using cqrs.Serialization;
using System.Collections.Generic;

namespace cqrs.Messaging
{
    public abstract class CommandBus : ICommandBus
    {
        private readonly IMessageSender sender;
        protected IMetadataProvider metadataProvider { get; private set; }
        protected ITextSerializer serializer { get; private set; }

        public CommandBus(IMessageSender sender, IMetadataProvider metadataProvider, ITextSerializer serializer)
        {
            this.sender = sender;
            this.metadataProvider = metadataProvider;
            this.serializer = serializer;
        }
        /// <summary>
        /// Sends the specified command.
        /// </summary>
        public void Send<TCommand>(Envelope<TCommand> command) where TCommand : ICommand
        {
            this.sender.Send(BuildMessage(command));
        }

        public void Send<TCommand>(IEnumerable<Envelope<TCommand>> commands) where TCommand : ICommand
        {
            foreach (var command in commands)
            {
                this.Send(command);
            }
        }

        public abstract IBrokeredMessage BuildMessage<TCommand>(Envelope<TCommand> command) where TCommand : ICommand;
    }
}

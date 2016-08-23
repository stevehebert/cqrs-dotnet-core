namespace cqrs.Messaging.Handling
{
    public abstract class CommandHandlerRegistry<T> : MessageProcessor<T>, ICommandHandlerRegistry
    {
        private readonly CommandDispatcher _commandDispatcher;

        protected CommandHandlerRegistry(IMessageReceiver<T> messageReceiver, ITextSerializer serializer, IDeliveryConfiguration deliveryConfiguration) : base(messageReceiver, serializer, deliveryConfiguration)
        {
            _commandDispatcher = new CommandDispatcher();
        }

        public void Register(ICommandHandler handler)
        {
            this._commandDispatcher.Register(handler);
        }

        protected override void ProcessMessage(IBrokeredMessage message)
        {
            this._commandDispatcher.ProcessMessage(message.Payload as ICommand,  message);
        }
    }
}
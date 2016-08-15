namespace cqrs.Messaging.Handling
{
    public interface ICommandHandlerRegistry
    {
        void Register(ICommandHandler handler);
    }
}

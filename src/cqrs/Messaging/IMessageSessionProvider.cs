namespace cqrs.Messaging
{
    public interface IMessageSessionProvider
    {
        string SessionId { get; }
    }
}

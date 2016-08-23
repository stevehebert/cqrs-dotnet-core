namespace cqrs.Messaging
{
    public interface IBrokeredMessage
    {
        string MessageId { get; }
        string CorrelationId { get; }
        string TraceId { get; }
        object Payload { get; }


        int DeliveryCount { get; set; }
    }
}

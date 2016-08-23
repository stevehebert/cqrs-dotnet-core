namespace cqrs.Messaging
{
    public class MessageReleaseAction
    {
        public static readonly MessageReleaseAction CompleteMessage = new MessageReleaseAction(MessageReleaseActionKind.Complete);
        public static readonly MessageReleaseAction AbandonMessage = new MessageReleaseAction(MessageReleaseActionKind.Abandon);

        protected MessageReleaseAction(MessageReleaseActionKind kind)
        {
            this.Kind = kind;
        }

        public MessageReleaseActionKind Kind { get; private set; }

        public string DeadLetterReason { get; private set; }

        public string DeadLetterDescription { get; private set; }

        public static MessageReleaseAction DeadLetterMessage(string reason, string description)
        {
            return new MessageReleaseAction(MessageReleaseActionKind.DeadLetter) { DeadLetterReason = reason, DeadLetterDescription = description };
        }
    }

    public enum MessageReleaseActionKind
    {
        Complete,
        Abandon,
        DeadLetter
    }
}

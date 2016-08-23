using System;

namespace cqrs.Messaging.Handling
{
    public interface ICommandHandler
    {
        void Handle(object command);
    }

    public abstract class BaseCommandHandler<T> : ICommandHandler where T : ICommand
    {
        public void Handle(object command)
        {
            TypedHandle((T) command);
        }

        public abstract void TypedHandle(T command);
    }
}


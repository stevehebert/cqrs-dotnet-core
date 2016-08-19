using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cqrs.Messaging
{
    public interface ICommandBus
    {
        void Send<TCommand>(Envelope<TCommand> command) where TCommand : ICommand;
        void Send<TCommand>(IEnumerable<Envelope<TCommand>> commands) where TCommand : ICommand;

        IBrokeredMessage BuildMessage<TCommand>(Envelope<TCommand> command) where TCommand : ICommand;
    }
}

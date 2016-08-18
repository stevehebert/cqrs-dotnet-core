using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cqrs.Messaging
{
    public interface ICommandBus
    {
        void Send(Envelope<ICommand> command);
        void Send(IEnumerable<Envelope<ICommand>> commands);

        IBrokeredMessage BuildMessage(Envelope<ICommand> command);
    }
}

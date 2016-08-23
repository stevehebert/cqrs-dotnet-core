using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace cqrs.Messaging.Handling
{
    public class CommandDispatcher
    {
        private readonly IDictionary<Type, ICommandHandler> _handlers = new Dictionary<Type, ICommandHandler>();

        public int HandlerCount => _handlers.Count;

        public void Register(ICommandHandler commandHandler)
        {
            var genericHandler = typeof(ICommandHandler<>);
            var supportedCommandTypes = commandHandler.GetType()
                .GetInterfaces()
                .Where(iface => IntrospectionExtensions.GetTypeInfo(iface).IsGenericType && iface.GetGenericTypeDefinition() == genericHandler)
                .Select(iface => iface.GetGenericArguments()[0])
                .ToList();

            if (_handlers.Keys.Any(registeredType => supportedCommandTypes.Contains(registeredType)))
                throw new ArgumentException("The command handled by the received handler already has a registered handler.");

            // Register this handler for each of he handled types.
            foreach (var commandType in supportedCommandTypes)
            {
                this._handlers.Add(commandType, commandHandler);
            }

        }

        public bool ProcessMessage(ICommand payload, IBrokeredMessage brokeredMessage)
        {
            var commandType = payload.GetType();
            ICommandHandler handler = null;

            if (this._handlers.TryGetValue(commandType, out handler))
            {
                ((dynamic)handler).Handle((dynamic)payload);
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
}
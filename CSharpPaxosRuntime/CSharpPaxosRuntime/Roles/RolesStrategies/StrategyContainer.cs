using System;
using System.Collections.Generic;
using CSharpPaxosRuntime.Messaging;

namespace CSharpPaxosRuntime.Roles.RolesStrategies
{
    public class StrategyContainer
    {
        private readonly Dictionary<Type, IMessageStrategy> _messageStrategies;

        public StrategyContainer()
        {
            _messageStrategies = new Dictionary<Type, IMessageStrategy>();
        }

        public void AddStrategy(Type t, IMessageStrategy strategy)
        {
            _messageStrategies.Add(t, strategy);
        }

        public void ExecuteStrategy(IMessage message, IPaxosActorState currentState)
        {
            var messageStrategy = RetrieveMessageStrategy(message);
            if (messageStrategy == null)
                return;

            var arg = new MessageStrategyExecuteArg<IMessage>
            {
                ActorState = currentState,
                Message = message
            };
            messageStrategy.Execute(arg);
        }

        public IMessageStrategy RetrieveMessageStrategy(IMessage message)
        {
            IMessageStrategy strategy;
            _messageStrategies.TryGetValue(message.GetType(), out strategy);
            return strategy;
        }
    }
}
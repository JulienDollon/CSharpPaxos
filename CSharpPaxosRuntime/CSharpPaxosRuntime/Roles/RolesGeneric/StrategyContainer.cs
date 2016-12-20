using System;
using System.Collections.Generic;
using CSharpPaxosRuntime.Messaging;

namespace CSharpPaxosRuntime.Roles.RolesGeneric
{
    public class StrategyContainer
    {
        public StrategyContainer()
        {
            this.messageStrategies = new Dictionary<Type, IMessageStrategy>();
        }

        public void AddStrategy(Type t, IMessageStrategy strategy)
        {
            this.messageStrategies.Add(t, strategy);
        }

        private readonly Dictionary<Type, IMessageStrategy> messageStrategies;

        public void ExecuteStrategy(IMessage message, IPaxosRoleState currentState)
        {
            IMessageStrategy messageStrategy = this.RetrieveMessageStrategy(message);
            if (messageStrategy == null)
            {
                return;
            }

            MessageStrategyExecuteArg<IMessage> arg = new MessageStrategyExecuteArg<IMessage>
            {
                RoleState = currentState,
                Message = message
            };
            messageStrategy.Execute(arg);
        }

        public IMessageStrategy RetrieveMessageStrategy(IMessage message)
        {
            IMessageStrategy strategy = null;
            this.messageStrategies.TryGetValue(message.GetType(), out strategy);
            return strategy;
        }
    }
}
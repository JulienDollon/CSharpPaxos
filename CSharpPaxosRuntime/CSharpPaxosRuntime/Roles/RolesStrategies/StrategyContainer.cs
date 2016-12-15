using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Roles.RolesStrategies;
using CSharpPaxosRuntime.RolesStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Roles.RolesStrategies
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

        private Dictionary<Type, IMessageStrategy> messageStrategies;

        public void ExecuteStrategy(IMessage message, IPaxosActorState currentState)
        {
            IMessageStrategy messageStrategy = this.RetrieveMessageStrategy(message);
            if (messageStrategy == null)
            {
                return;
            }

            MessageStrategyExecuteArg<IMessage> arg = new MessageStrategyExecuteArg<IMessage>();
            arg.ActorState = currentState;
            arg.Message = message;
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
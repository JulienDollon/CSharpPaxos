using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.RolesGeneric;

namespace CSharpPaxosRuntime.Roles.Leader.LeaderStrategies
{
    public class SendSolicitateBallotRequestStrategy : IMessageStrategy
    {
        private readonly IMessageBroker broker;
        public SendSolicitateBallotRequestStrategy(IMessageBroker broker)
        {
            this.broker = broker;
        }

        public void Execute(MessageStrategyExecuteArg<IMessage> obj)
        {
            if (!(obj.Message is SolicitateBallotRequest))
            {
                throw new MessageStrategyException("This strategy shouldn't be invoked with this message type");
            }

            SolicitateBallotRequest request = obj.Message as SolicitateBallotRequest;
            sendRequestToAcceptors(obj.RoleState as LeaderState, request);
        }

        private void sendRequestToAcceptors(LeaderState state, SolicitateBallotRequest objMessage)
        {
            state.BallotRequestPendingDecisionByAcceptors.Clear();
            foreach (MessageSender acceptor in state.Acceptors)
            {
                state.BallotRequestPendingDecisionByAcceptors.Add(acceptor);
                broker.SendMessage(acceptor.UniqueId, objMessage);
            }
        }
    }
}
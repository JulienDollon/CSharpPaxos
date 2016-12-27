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
    public class SendVoteRequestToAcceptors : IMessageStrategy
    {
        private readonly IMessageBroker broker;
        public SendVoteRequestToAcceptors(IMessageBroker broker)
        {
            this.broker = broker;
        }

        public void Execute(MessageStrategyExecuteArg<IMessage> obj)
        {
            if (!(obj.Message is VoteRequest))
            {
                throw new MessageStrategyException("This strategy shouldn't be invoked with this message type");
            }

            VoteRequest request = obj.Message as VoteRequest;
            sendRequestToAcceptors(obj.RoleState as LeaderState, request);
        }

        private void sendRequestToAcceptors(LeaderState state, VoteRequest objMessage)
        {
            state.VoteRequestPendingDecisionPerSlot[objMessage.SlotNumber] = new List<MessageSender>();
            foreach (MessageSender acceptor in state.Acceptors)
            {
                state.VoteRequestPendingDecisionPerSlot[objMessage.SlotNumber].Add(acceptor);
                broker.SendMessage(acceptor.UniqueId, objMessage);
            }
        }
    }
}
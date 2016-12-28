using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.RolesGeneric;

namespace CSharpPaxosRuntime.Roles.Replica.ReplicaStrategies
{
    public class SendProposalRequestToLeaders : IMessageStrategy
    {
        private readonly IMessageBroker broker;
        public SendProposalRequestToLeaders(IMessageBroker broker)
        {
            this.broker = broker;
        }

        public void Execute(MessageStrategyExecuteArg<IMessage> obj)
        {
            if (!(obj.Message is ClientRequest))
            {
                throw new MessageStrategyException("This strategy shouldn't be invoked with this message type");
            }

            ClientRequest request = obj.Message as ClientRequest;
            ReplicaState state = obj.RoleState as ReplicaState;

            sendProposalsToLeaders(request, state);
        }

        private void sendProposalsToLeaders(ClientRequest request, ReplicaState state)
        {
            state.ProposalsRequestsBySlotId.Add(state.FirstUnusedSlot, request);
            state.ClientsPendingResponseBySlotId.Add(state.FirstUnusedSlot, request);

            ProposalRequest proposal = new ProposalRequest();
            proposal.SlotNumber = state.FirstUnusedSlot;
            proposal.Command = request.Command;
            proposal.MessageSender = state.MessageSender;

            foreach (MessageSender leader in state.Leaders)
            {
                this.broker.SendMessage(leader.UniqueId, proposal);
            }

            state.FirstUnusedSlot++;
        }
    }
}
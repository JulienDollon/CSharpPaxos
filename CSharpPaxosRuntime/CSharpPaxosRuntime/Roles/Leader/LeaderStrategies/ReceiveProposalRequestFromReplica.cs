using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.RolesGeneric;

namespace CSharpPaxosRuntime.Roles.Leader.LeaderStrategies
{
    public class ReceiveProposalRequestFromReplica : IMessageStrategy
    {
        public event EventHandler OnProposalReceived;
        public void Execute(MessageStrategyExecuteArg<IMessage> obj)
        {
            if (!(obj.Message is ProposalRequest))
            {
                throw new MessageStrategyException("This strategy shouldn't be invoked with this message type");
            }
            ProposalRequest request = obj.Message as ProposalRequest;
            LeaderState state = obj.RoleState as LeaderState;;
            storeProposal(request, state);
            OnProposalReceived?.Invoke(this, EventArgs.Empty);
        }

        private void storeProposal(ProposalRequest request, LeaderState state)
        {
            if (!leadersOrReplicasAskedVoteForThisSlot(request.SlotNumber, state))
            {
                state.ProposalsBySlotId.Add(request.SlotNumber, request.Command);
            }
        }

        private bool leadersOrReplicasAskedVoteForThisSlot(int requestSlotNumber, LeaderState state)
        {
            return state.ProposalsBySlotId.ContainsKey(requestSlotNumber);
        }
    }
}
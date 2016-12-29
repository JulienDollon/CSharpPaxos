using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.RolesGeneric;

namespace CSharpPaxosRuntime.Roles.Replica.ReplicaStrategies
{
    public class ReceiveProposalDecisionFromLeader : IMessageStrategy
    {
        public event EventHandler<ClientRequest> OnDecisionApproved;
        public event EventHandler<ClientRequest> OnDecisionRejected;
        public void Execute(MessageStrategyExecuteArg<IMessage> obj)
        {
            if (!(obj.Message is ProposalDecision))
            {
                throw new MessageStrategyException("This strategy shouldn't be invoked with this message type");
            }

            ProposalDecision decision = obj.Message as ProposalDecision;
            ReplicaState state = obj.RoleState as ReplicaState;

            storeDecision(decision, state);
            if (proposalHasBeenApproved(decision, state))
            {
                ClientRequest clientRequest = cleanProposal(decision, state);
                OnDecisionApproved?.Invoke(this, clientRequest);
            }
            else
            {
                ClientRequest clientRequest = null;
                if (replicaIsProposalEmitter(decision, state))
                {
                    temporaryRemoveClientPendingResponseUntilNextRetry(decision, state);
                    clientRequest = cleanProposal(decision, state);
                }

                OnDecisionRejected?.Invoke(this, clientRequest);
            }
        }

        private bool replicaIsProposalEmitter(ProposalDecision decision, ReplicaState state)
        {
            return state.ProposalsRequestsBySlotId.ContainsKey(decision.SlotNumber);
        }

        private void temporaryRemoveClientPendingResponseUntilNextRetry(ProposalDecision decision, ReplicaState state)
        {
            state.ClientsPendingResponseBySlotId.Remove(decision.SlotNumber);
        }

        private bool proposalHasBeenApproved(ProposalDecision decision, ReplicaState state)
        {
            return state.ProposalsRequestsBySlotId.ContainsKey(decision.SlotNumber) &&
                   state.DecisionsBySlotId.ContainsKey(decision.SlotNumber) &&
                   state.ProposalsRequestsBySlotId[decision.SlotNumber].Command == state.DecisionsBySlotId[decision.SlotNumber];
        }

        private void storeDecision(ProposalDecision decision, ReplicaState state)
        {
            if (!state.DecisionsBySlotId.ContainsKey(decision.SlotNumber))
            {
                state.DecisionsBySlotId.Add(decision.SlotNumber, decision.Command);
            }
        }

        private ClientRequest cleanProposal(ProposalDecision decision, ReplicaState state)
        {
            ClientRequest request = state.ProposalsRequestsBySlotId[decision.SlotNumber];
            state.ProposalsRequestsBySlotId.Remove(decision.SlotNumber);
            return request;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.RolesGeneric;

namespace CSharpPaxosRuntime.Roles.Leader.LeaderStrategies
{
    public class ReceiveVoteResponseStrategy : IMessageStrategy
    {
        public event EventHandler<IMessage> OnApprovalElected;
        public event EventHandler OnApprovalPreempted;

        public void Execute(MessageStrategyExecuteArg<IMessage> obj)
        {
            if (!(obj.Message is VoteResponse))
            {
                throw new MessageStrategyException("This strategy shouldn't be invoked with this message type");
            }

            VoteResponse response = obj.Message as VoteResponse;
            LeaderState state = obj.RoleState as LeaderState;

            state.VoteRequestPendingDecisionByAcceptors.Remove(response.MessageSender);

            if (isBallotValid(response.BallotNumber, state.BallotNumber))
            {
                if (isElected(response, state))
                {
                    OnApprovalElected?.Invoke(this, response);
                }
            }
            else
            {
                OnApprovalPreempted?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool isElected(VoteResponse response, LeaderState state)
        {
            return state.VoteRequestPendingDecisionByAcceptors.Count < state.Acceptors.Count / 2;
        }

        private bool isBallotValid(BallotNumber responseBallotNumber, BallotNumber stateBallotNumber)
        {
            return responseBallotNumber == stateBallotNumber;
        }
    }
}
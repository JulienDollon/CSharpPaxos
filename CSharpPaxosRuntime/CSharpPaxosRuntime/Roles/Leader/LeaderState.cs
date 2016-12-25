using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Environment;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.Properties;

namespace CSharpPaxosRuntime.Roles.Leader
{
    public class LeaderState : IPaxosRoleState,
                               IBallotNumberProperty,
                               IBallotAdoptedProperty
    {
        public LeaderState()
        {
            BallotRequestPendingDecisionByAcceptors = new List<MessageSender>();
            VoteRequestPendingDecisionByAcceptors = new List<MessageSender>();
            ValuesAcceptedByAcceptors = new List<VoteDecision>();
            ProposalsBySlotId = new Dictionary<int, ICommand>();
        }

        public MessageSender MessageSender { get; set; }
        public BallotNumber BallotNumber { get; set; }
        public BallotStatus BallotStatus { get; set; }
        public List<MessageSender> Acceptors { get; set; }
        public List<MessageSender> Replicas { get; set; }
        public List<MessageSender> BallotRequestPendingDecisionByAcceptors { get; set; }
        public List<MessageSender> VoteRequestPendingDecisionByAcceptors { get; set; }
        public List<VoteDecision> ValuesAcceptedByAcceptors { get; set; }
        public Dictionary<int, ICommand> ProposalsBySlotId { get; set; }
    }
}
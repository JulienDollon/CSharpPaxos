using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            WaitingFor = new List<MessageSender>();
            ValuesAcceptedByAcceptors = new List<VoteDecision>();
        }

        public MessageSender MessageSender { get; set; }
        public BallotNumber BallotNumber { get; set; }
        public BallotStatus BallotStatus { get; set; }
        public List<MessageSender> Acceptors { get; set; }
        public List<MessageSender> WaitingFor { get; set; }
        public List<VoteDecision> ValuesAcceptedByAcceptors { get; set; }
    }
}
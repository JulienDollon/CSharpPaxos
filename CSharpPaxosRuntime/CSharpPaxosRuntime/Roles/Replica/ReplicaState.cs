using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;

namespace CSharpPaxosRuntime.Roles.Replica
{
    public class ReplicaState : IPaxosRoleState
    {
        public ReplicaState()
        {
            this.ProposalsRequestsBySlotId = new Dictionary<int, ClientRequest>();
            this.DecisionsBySlotId = new Dictionary<int, ICommand>();
            this.ClientsPendingResponseBySlotId = new Dictionary<int, ClientRequest>();
            FirstUnusedSlot = 0;
            LastAppliedSlotToStateMachine = 0;
        }

        public MessageSender MessageSender { get; set; }
        public Dictionary<int, ClientRequest> ProposalsRequestsBySlotId { get; set; }
        public Dictionary<int, ICommand> DecisionsBySlotId { get; set; }
        public Dictionary<int, ClientRequest> ClientsPendingResponseBySlotId { get; set; }
        public int FirstUnusedSlot { get; set; }
        public List<MessageSender> Leaders { get; set; }
        public IStateMachine StateMachine { get; set; }
        public int LastAppliedSlotToStateMachine { get; set; }
    }
}
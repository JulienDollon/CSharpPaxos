using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Roles
{
    public class AcceptorState : IPaxosActorState
    {
        public AcceptorState()
        {
            this.AcceptedDecisions = new Decisions();
        }

        public int BallotNumber { get; set; }

        public MessageSender MessageSender { get; set; }

        public Decisions AcceptedDecisions { get; set; }
    }
}
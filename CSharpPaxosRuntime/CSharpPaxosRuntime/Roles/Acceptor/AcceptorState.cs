using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Roles.Acceptor
{
    public class AcceptorState : IPaxosActorState, IBallotNumberProperty
    {
        public AcceptorState()
        {
            this.AcceptedDecisions = new VoteDecisions();
        }

        public BallotNumber BallotNumber { get; set; }

        public MessageSender MessageSender { get; set; }

        public VoteDecisions AcceptedDecisions { get; set; }
    }
}
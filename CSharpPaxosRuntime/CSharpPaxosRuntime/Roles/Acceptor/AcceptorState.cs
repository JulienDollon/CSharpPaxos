using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.Properties;

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
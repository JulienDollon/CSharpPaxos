using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Roles
{
    public class AcceptorState : IPaxosActorState
    {
        public AcceptorState()
        {
            AcceptedDecisions = new VoteDecisions();
        }

        public int BallotNumber { get; set; }

        public MessageSender MessageSender { get; set; }

        public VoteDecisions AcceptedDecisions { get; set; }
    }
}
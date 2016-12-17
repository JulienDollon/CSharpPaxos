using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
{
    public class SolicitateBallotResponse : IMessage, IBallotNumberProperty, IDecisionsProperty
    {
        public BallotNumber BallotNumber
        {
            get;
            set;
        }

        public VoteDecisions Decisions
        {
            get;
            set;
        }

        public MessageSender MessageSender
        {
            get;
            set;
        }
    }
}
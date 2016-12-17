using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models.Properties;

namespace CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes
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
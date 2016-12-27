using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models.Properties;

namespace CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes
{
    public class SolicitateBallotResponse : IMessage, IBallotNumberProperty
    {
        public BallotNumber BallotNumber
        {
            get;
            set;
        }

        public VoteResponse Decision
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
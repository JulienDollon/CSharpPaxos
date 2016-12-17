using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models.Properties;

namespace CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes
{
    public class SolicitateBallotRequest : IMessage, IBallotNumberProperty
    {
        public BallotNumber BallotNumber
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
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
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
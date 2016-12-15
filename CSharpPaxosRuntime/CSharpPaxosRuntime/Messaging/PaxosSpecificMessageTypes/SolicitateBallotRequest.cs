using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
{
    public class SolicitateBallotRequest : IMessage, IBallotNumberProperty
    {
        public int BallotNumber
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
using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
{
    public class VoteResponse : IMessage, IBallotNumberProperty, ISlotNumberProperty
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

        public int SlotNumber
        {
            get;
            set;
        }
    }
}
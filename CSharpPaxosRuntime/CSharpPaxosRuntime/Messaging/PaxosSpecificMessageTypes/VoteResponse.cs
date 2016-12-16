using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
{
    public class VoteResponse : IMessage, IBallotNumberProperty, ISlotNumberProperty, IVoteStatusProperty
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

        public VoteStatus VoteStatus
        {
            get;
            set;
        }
    }
}
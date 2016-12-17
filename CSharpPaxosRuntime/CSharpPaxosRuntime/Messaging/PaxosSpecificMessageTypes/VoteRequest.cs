using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
{
    public class VoteRequest : IMessage, IBallotNumberProperty, ISlotNumberProperty, ICommandProperty
    {
        public BallotNumber BallotNumber
        {
            get;
            set;
        }

        public ICommand Command
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
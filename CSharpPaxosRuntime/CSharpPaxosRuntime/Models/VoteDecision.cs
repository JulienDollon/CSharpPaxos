using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Models
{
    public class VoteDecision : ISlotNumberProperty, IBallotNumberProperty, ICommandProperty
    {
        public int BallotNumber { get; set; }

        public ICommand Command { get; set; }

        public int SlotNumber { get; set; }
    }
}
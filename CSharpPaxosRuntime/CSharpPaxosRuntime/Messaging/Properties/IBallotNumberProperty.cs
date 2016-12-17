using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Messaging.Properties
{
    public interface IBallotNumberProperty
    {
        BallotNumber BallotNumber { get; set; }
    }
}
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Messaging.Properties
{
    public interface IVoteStatusProperty
    {
        VoteStatus VoteStatus { get; set; }
    }
}
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Messaging.Properties
{
    public interface IDecisionsProperty
    {
        VoteDecisions Decisions { get; set; }
    }
}
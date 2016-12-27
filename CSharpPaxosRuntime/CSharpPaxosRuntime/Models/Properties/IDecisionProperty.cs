using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;

namespace CSharpPaxosRuntime.Models.Properties
{
    public interface IDecisionProperty
    {
        IDecision Decision { get; set; }
    }
}
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Messaging.Properties
{
    public interface ICommandProperty
    {
        ICommand Command { get; set; }
    }
}
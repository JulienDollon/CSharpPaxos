using CSharpPaxosRuntime.Messaging.Bus;

namespace CSharpPaxosRuntime.Models.Properties
{
    public interface IMessageReceiverProperty
    {
        IMessageReceiver MessageReceiver { get; set; }
    }
}
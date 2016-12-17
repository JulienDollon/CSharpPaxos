using CSharpPaxosRuntime.Messaging.Bus;

namespace CSharpPaxosRuntime.Messaging.Properties
{
    public interface IMessageReceiverProperty
    {
        IMessageReceiver MessageReceiver { get; set; }
    }
}
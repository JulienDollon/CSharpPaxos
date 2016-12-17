using CSharpPaxosRuntime.Messaging.Bus;

namespace CSharpPaxosRuntime.Models.Properties
{
    public interface IMessageSenderProperty
    {
        MessageSender MessageSender { get; set; }
    }
}
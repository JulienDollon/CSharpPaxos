using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;

namespace CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes
{
    public class ReconfigRequest : IMessage
    {
        public MessageSender MessageSender
        {
            get;
            set;
        }
    }
}
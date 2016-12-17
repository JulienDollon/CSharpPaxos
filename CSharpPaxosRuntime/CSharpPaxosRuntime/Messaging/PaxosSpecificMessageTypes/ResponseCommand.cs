using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
{
    public class ResponseCommand : IMessage
    {
        public MessageSender MessageSender
        {
            get;
            set;
        }
    }
}
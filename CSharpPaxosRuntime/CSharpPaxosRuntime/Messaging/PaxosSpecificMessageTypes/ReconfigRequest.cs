using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
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
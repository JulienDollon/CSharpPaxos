using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
{
    public class ClientRequest : IMessage
    {
        public MessageSender MessageSender
        {
            get;
            set;
        }
    }
}
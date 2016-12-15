using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
{
    public class PreemptedResponse : IMessage
    {
        public MessageSender MessageSender
        {
            get;
            set;
        }
    }
}
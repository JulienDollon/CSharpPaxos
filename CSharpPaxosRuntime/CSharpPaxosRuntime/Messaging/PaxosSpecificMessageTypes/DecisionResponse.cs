using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
{
    public class DecisionResponse : IMessage
    {
        public MessageSender MessageSender
        {
            get;
            set;
        }
    }
}
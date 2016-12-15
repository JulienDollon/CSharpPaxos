using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
{
    public class ProposalRequest : IMessage
    {
        public MessageSender MessageSender
        {
            get;
            set;
        }
    }
}
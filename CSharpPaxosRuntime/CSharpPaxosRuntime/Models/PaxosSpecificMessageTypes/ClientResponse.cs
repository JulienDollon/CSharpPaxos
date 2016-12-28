using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models.Properties;

namespace CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes
{
    public class ClientResponse : IMessage, ICommandProperty, IVoteStatusProperty, IMessageSenderProperty
    {
        public VoteStatus VoteStatus { get; set; }
        public ICommand Command { get; set; }
        public MessageSender MessageSender { get; set; }
    }
}
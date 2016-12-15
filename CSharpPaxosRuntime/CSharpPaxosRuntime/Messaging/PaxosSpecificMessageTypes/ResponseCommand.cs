using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
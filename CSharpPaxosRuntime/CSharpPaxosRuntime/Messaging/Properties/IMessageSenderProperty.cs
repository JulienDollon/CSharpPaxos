using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Messaging
{
    public interface IMessageSenderProperty
    {
        MessageSender MessageSender { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Messaging
{
    public interface IMessageReceiver
    {
        void ReceiveMessage(IMessage message);
        IMessage GetLastMessage();
    }
}
using CSharpPaxosRuntime.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Messaging
{
    public class MessageReceiver : IMessageReceiver
    {
        public MessageReceiver()
        {
            this.messages = new FixedSizedQueue<IMessage>();
        }

        private FixedSizedQueue<IMessage> messages;

        public void ReceiveMessage(IMessage message)
        {
            messages.Enqueue(message);
        }

        public IMessage GetLastMessage()
        {
            return messages.Dequeue();
        }
    }
}
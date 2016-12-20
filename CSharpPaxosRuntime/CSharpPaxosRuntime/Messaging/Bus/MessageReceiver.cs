using CSharpPaxosRuntime.Utils;
using CSharpPaxosRuntime.Utils.DataStructures;

namespace CSharpPaxosRuntime.Messaging.Bus
{
    public class MessageReceiver : IMessageReceiver
    {
        public MessageReceiver()
        {
            this.messages = new FixedSizedQueue<IMessage>();
        }

        private readonly FixedSizedQueue<IMessage> messages;

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
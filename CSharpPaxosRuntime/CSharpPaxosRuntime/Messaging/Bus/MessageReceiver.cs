using CSharpPaxosRuntime.Utils;

namespace CSharpPaxosRuntime.Messaging.Bus
{
    public class MessageReceiver : IMessageReceiver
    {
        public MessageReceiver()
        {
            _messages = new FixedSizedQueue<IMessage>();
        }

        private readonly FixedSizedQueue<IMessage> _messages;

        public void ReceiveMessage(IMessage message)
        {
            _messages.Enqueue(message);
        }

        public IMessage GetLastMessage()
        {
            return _messages.Dequeue();
        }
    }
}
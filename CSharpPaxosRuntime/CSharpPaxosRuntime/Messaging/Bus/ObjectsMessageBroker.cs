using System.Collections.Generic;

namespace CSharpPaxosRuntime.Messaging.Bus
{
    public class ObjectsMessageBroker : IMessageBroker
    {
        public ObjectsMessageBroker()
        {
            this.hashtableOfReceiver = new Dictionary<string, IMessageReceiver>();
        }

        private readonly Dictionary<string, IMessageReceiver> hashtableOfReceiver;

        public void AddReceiver(string receiverAddress, IMessageReceiver instance)
        {
            this.hashtableOfReceiver.Add(receiverAddress, instance);
        }

        public void RemoveReceiver(string receiverAddress)
        {
            this.hashtableOfReceiver.Remove(receiverAddress);
        }

        public bool SendMessage(string receiverAddress, IMessage message)
        {
            IMessageReceiver receiver = null;
            bool success = this.hashtableOfReceiver.TryGetValue(receiverAddress, out receiver);
            receiver?.ReceiveMessage(message);
            return success;
        }
    }
}
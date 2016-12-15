using System.Collections.Generic;

namespace CSharpPaxosRuntime.Messaging.Bus
{
    public class ObjectsMessageBroker : IMessageBroker
    {
        public ObjectsMessageBroker()
        {
            _hashtableOfReceiver = new Dictionary<string, IMessageReceiver>();
        }

        private readonly Dictionary<string, IMessageReceiver> _hashtableOfReceiver;

        public void AddReceiver(string receiverAddress, IMessageReceiver instance)
        {
            _hashtableOfReceiver.Add(receiverAddress, instance);
        }

        public void RemoveReceiver(string receiverAddress)
        {
            _hashtableOfReceiver.Remove(receiverAddress);
        }

        public bool SendMessage(string receiverAddress, IMessage message)
        {
            IMessageReceiver receiver;
            bool success = _hashtableOfReceiver.TryGetValue(receiverAddress, out receiver);
            receiver?.ReceiveMessage(message);
            return success;
        }
    }
}
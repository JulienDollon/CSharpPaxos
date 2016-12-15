namespace CSharpPaxosRuntime.Messaging.Bus
{
    public interface IMessageBroker
    {
        void AddReceiver(string receiverAddress, IMessageReceiver instance);
        bool SendMessage(string receiverAddress, IMessage message);
        void RemoveReceiver(string receiverAddress);
    }
}
namespace CSharpPaxosRuntime.Messaging.Bus
{

    public interface IMessageReceiver
    {
        void ReceiveMessage(IMessage message);
        IMessage GetLastMessage();
    }
}
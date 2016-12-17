using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Roles
{
    internal interface IPaxosActor : IMessageReceiverProperty
    {
        IMessageBroker MessageBroker { get; }
        IPaxosActorState ActorState { get; }
        void Start();
        void Stop();
    }
}
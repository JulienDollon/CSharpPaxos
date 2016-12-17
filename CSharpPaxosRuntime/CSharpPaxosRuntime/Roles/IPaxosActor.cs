using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models.Properties;

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
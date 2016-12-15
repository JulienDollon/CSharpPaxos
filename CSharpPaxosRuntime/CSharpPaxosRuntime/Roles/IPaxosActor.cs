using CSharpPaxosRuntime.Messaging.Bus;

namespace CSharpPaxosRuntime.Roles
{
    internal interface IPaxosActor
    {
        IMessageReceiver MessageReceiver { get; }
        IPaxosActorState ActorState { get; }
        void Start();
        void Stop();
    }
}
using CSharpPaxosRuntime.Messaging;

namespace CSharpPaxosRuntime.Roles.RolesGeneric
{
    public class MessageStrategyExecuteArg<T> where T : IMessage
    {
        public T Message { get; set; }
        public IPaxosActorState ActorState { get; set; }
    }
}
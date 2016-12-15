using CSharpPaxosRuntime.Messaging;

namespace CSharpPaxosRuntime.Roles.RolesStrategies
{
    public class MessageStrategyExecuteArg<T> where T : IMessage
    {
        public T Message { get; set; }
        public IPaxosActorState ActorState { get; set; }
    }
}
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models.Properties;

namespace CSharpPaxosRuntime.Roles
{
    internal interface IPaxosRole : IMessageReceiverProperty
    {
        IMessageBroker MessageBroker { get; }
        IPaxosRoleState RoleState { get; }
        void Start();
        void Stop();
    }
}
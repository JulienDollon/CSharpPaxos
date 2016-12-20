using System;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Utils;

namespace CSharpPaxosRuntime.Roles.RolesGeneric
{
    public interface IPaxosRoleLoopMessageListener : IStrategy
    {
        void Initialize(IMessageReceiver MessageReceiver,
                        Action<IMessage> OnMessageDequeued,
                        IMessageBroker messageBroker,
                        MessageSender owner);
        bool KeepRunning { get; set; }
    }
}
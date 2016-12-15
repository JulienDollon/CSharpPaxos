using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Utils;
using System;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Roles
{
    public interface IPaxosActorLoopMessageListener : IStrategy
    {
        void Initialize(IMessageReceiver messageReceiver,
                        Action<IMessage> onMessageDequeued,
                        IMessageBroker messageBroker,
                        MessageSender owner);
        bool KeepRunning { get; set; }
    }
}
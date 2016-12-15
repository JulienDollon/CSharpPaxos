using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Utils;
using System;

namespace CSharpPaxosRuntime.Roles
{
    public interface IPaxosActorLoopMessageListener : IStrategy
    {
        void Initialize(IMessageReceiver MessageReceiver,
                        Action<IMessage> OnMessageDequeued,
                        IMessageBroker messageBroker,
                        MessageSender owner);
        bool KeepRunning { get; set; }
    }
}
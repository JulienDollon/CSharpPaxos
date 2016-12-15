using CSharpPaxosRuntime.Messaging;
using System;
using System.Threading;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Roles
{
    public class PaxosActorLoopMessageListener : IPaxosActorLoopMessageListener
    {
        private IMessageReceiver _messageReceiver;
        private Action<IMessage> _onMessageDequeued;
        private IMessageBroker _messageBroker;

        public void Initialize(IMessageReceiver messageReceiver, 
                               Action<IMessage> onMessageDequeued,
                               IMessageBroker messageBroker,
                               MessageSender owner)
        {
            _messageReceiver = messageReceiver;
            _onMessageDequeued = onMessageDequeued;
            _messageBroker = messageBroker;
            InitializeMessageBrokerSubscription(owner.UniqueId);
        }

        private void InitializeMessageBrokerSubscription(string uniqueId)
        {
            _messageBroker.AddReceiver(uniqueId, _messageReceiver);
        }

        public bool KeepRunning { get; set; }

        public void Execute()
        {
            KeepRunning = true;
            SpinWait waiter = new SpinWait();
            while (KeepRunning)
            {
                IMessage lastMessage = _messageReceiver.GetLastMessage();
                if (lastMessage != null)
                {
                    _onMessageDequeued.Invoke(lastMessage);
                }

                waiter.SpinOnce();
            }
        }
    }
}
using System;
using System.Threading;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;

namespace CSharpPaxosRuntime.Roles.RolesGeneric
{
    public class PaxosRoleLoopMessageListener : IPaxosRoleLoopMessageListener
    {
        private IMessageReceiver messageReceiver;
        private Action<IMessage> onMessageDequeued;
        private IMessageBroker messageBroker;

        public void Initialize(IMessageReceiver MessageReceiver, 
                               Action<IMessage> OnMessageDequeued,
                               IMessageBroker messageBroker,
                               MessageSender owner)
        {
            this.messageReceiver = MessageReceiver;
            this.onMessageDequeued = OnMessageDequeued;
            this.messageBroker = messageBroker;
            initializeMessageBrokerSubscription(owner.UniqueId);
        }

        private void initializeMessageBrokerSubscription(string uniqueId)
        {
            this.messageBroker.AddReceiver(uniqueId, this.messageReceiver);
        }

        public bool KeepRunning { get; set; }

        public void Execute()
        {
            KeepRunning = true;
            SpinWait waiter = new SpinWait();
            while (KeepRunning)
            {
                IMessage lastMessage = this.messageReceiver.GetLastMessage();
                if (lastMessage != null)
                {
                    this.onMessageDequeued.Invoke(lastMessage);
                }

                waiter.SpinOnce();
            }
        }
    }
}
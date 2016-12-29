using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Roles.Acceptor;
using CSharpPaxosRuntime.Roles.RolesGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Roles.Factories
{
    public class AcceptorFactory
    {
        public Acceptor.Acceptor BuildInstance(IMessageBroker broker)
        {
            IMessageReceiver receiver = new MessageReceiver();
            IPaxosRoleLoopMessageListener listener = new PaxosRoleLoopMessageListener();
            Acceptor.Acceptor instance = new Acceptor.Acceptor(receiver, listener, broker);
            broker.AddReceiver(instance.RoleState.MessageSender.UniqueId, receiver);
            return instance;
        }
    }
}
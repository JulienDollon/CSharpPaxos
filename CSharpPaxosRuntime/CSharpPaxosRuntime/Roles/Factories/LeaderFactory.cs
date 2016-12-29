using CSharpPaxosRuntime.Environment;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Roles.RolesGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Roles.Factories
{
    public class LeaderFactory
    {
        public Leader.Leader BuildInstance(IMessageBroker broker, List<Acceptor.Acceptor> acceptors, List<Replica.Replica> replicas)
        {
            IMessageReceiver receiver = new MessageReceiver();
            IPaxosRoleLoopMessageListener listener = new PaxosRoleLoopMessageListener();
            List<MessageSender> replicaMsgSender = (from rep in replicas select rep.RoleState.MessageSender).ToList();
            List<MessageSender> acceptorMsgSender = (from acp in acceptors select acp.RoleState.MessageSender).ToList();
            Leader.Leader instance = new Leader.Leader(receiver, listener, broker, acceptorMsgSender, replicaMsgSender, new TimeOut());
            broker.AddReceiver(instance.RoleState.MessageSender.UniqueId, receiver);
            return instance;
        }
    }
}
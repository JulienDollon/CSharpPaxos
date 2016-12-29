using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Roles.RolesGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Roles.Factories
{
    public class ReplicaFactory
    {
        public void StartInstance(Replica.Replica instance, List<Leader.Leader> leaders)
        {
            List<MessageSender> leadersMsgSender = (from leader in leaders select leader.RoleState.MessageSender).ToList();
            (instance.RoleState as Replica.ReplicaState).Leaders = leadersMsgSender;
            instance.Start();
        }

        public Replica.Replica BuildInstance(IMessageBroker broker, IStateMachine stateMachine)
        {
            IMessageReceiver receiver = new MessageReceiver();
            IPaxosRoleLoopMessageListener listener = new PaxosRoleLoopMessageListener();
            Replica.Replica instance = new Replica.Replica(receiver, listener, broker, stateMachine);
            broker.AddReceiver(instance.RoleState.MessageSender.UniqueId, receiver);
            return instance;
        }
    }
}
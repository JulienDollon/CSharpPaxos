using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Environment;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Roles;
using CSharpPaxosRuntime.Roles.Acceptor;
using CSharpPaxosRuntime.Roles.Leader;
using CSharpPaxosRuntime.Roles.RolesGeneric;
using CSharpPaxosRuntime.Tests.Roles.AcceptorRole.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpPaxosRuntime.Tests.Roles.LeaderRole.IntegrationTests
{
    public static class LeaderTestUtil
    {
        public static Leader GetLeaderWithNoMocks(List<MessageSender> acceptors, List<MessageSender> replicas, IMessageBroker broker, ITimeOut timeout)
        {
            IMessageReceiver receiver = new MessageReceiver();

            PaxosRoleLoopMessageListener loopListener = new PaxosRoleLoopMessageListener();
            Leader leader = new Leader(receiver, loopListener, broker, acceptors, replicas, timeout);

            return leader;
        }

        public static Leader GetAcceptorWithNoMocksRunning(List<MessageSender> acceptors, List<MessageSender> replicas, IMessageBroker broker, ITimeOut timeout, BallotNumber defaultBallotNumber = null)
        {
            Leader leader = LeaderTestUtil.GetLeaderWithNoMocks(acceptors, replicas, broker, timeout);

            if (defaultBallotNumber != null)
            {
                LeaderState leaderState = leader.RoleState as LeaderState;
                leaderState.BallotNumber = defaultBallotNumber;
            }

            Task t = Task.Run(() => {
                leader.Start();
            });

            return leader;
        }

        public static List<Acceptor> GenerateAcceptors(IMessageBroker broker)
        {
            Acceptor acceptor1 = AcceptorTestUtil.GetAcceptorWithNoMocksRunning(broker);
            Acceptor acceptor2 = AcceptorTestUtil.GetAcceptorWithNoMocksRunning(broker);
            Acceptor acceptor3 = AcceptorTestUtil.GetAcceptorWithNoMocksRunning(broker);
            List<Acceptor> acceptors = new List<Acceptor>();
            acceptors.Add(acceptor1);
            acceptors.Add(acceptor2);
            acceptors.Add(acceptor3);
            return acceptors;
        }

        public static List<MessageSender> AcceptorInstanceListToMessageSenders(List<Acceptor> acceptors)
        {
            List<MessageSender> senders = new List<MessageSender>();
            foreach (Acceptor acceptor in acceptors)
            {
                senders.Add(acceptor.RoleState.MessageSender);
            }
            return senders;
        }

        public static List<MessageSender> GenerateFakeReplicas(Action actionOnMessage, IMessageBroker broker)
        {
            FakeMessageReceiver receiver = new FakeMessageReceiver();
            receiver.OnMessageReceived += (sender, args) => actionOnMessage.Invoke();
            MessageSender fakeReplica = new MessageSender();
            fakeReplica.UniqueId = "JAKOBI";
            broker.AddReceiver(fakeReplica.UniqueId, receiver);
            
            List<MessageSender> replicas = new List<MessageSender>();
            replicas.Add(fakeReplica);
            return replicas;
        }
    }
}
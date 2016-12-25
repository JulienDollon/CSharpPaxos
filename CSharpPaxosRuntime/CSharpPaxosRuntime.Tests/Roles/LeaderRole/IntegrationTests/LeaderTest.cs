using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Environment;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Roles.Acceptor;
using CSharpPaxosRuntime.Roles.Leader;
using CSharpPaxosRuntime.Tests.Roles.AcceptorRole.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpPaxosRuntime.Tests.Roles.LeaderRole.IntegrationTests
{
    [TestClass]
    public class LeaderTest
    {
        private static readonly IMessageBroker MessageBroker = new ObjectsMessageBroker();

        [TestMethod]
        public void LeaderSolicitateBallotAndAcquireHighestBallot()
        {
            List<Acceptor> acceptors = LeaderTestUtil.GenerateAcceptors(MessageBroker);
            List<MessageSender> acceptorsSenders = LeaderTestUtil.AcceptorInstanceListToMessageSenders(acceptors);
            List<MessageSender> replicas = LeaderTestUtil.GenerateFakeReplicas(null, MessageBroker);
            Leader leader = LeaderTestUtil.GetAcceptorWithNoMocksRunning(acceptorsSenders, replicas, MessageBroker, new TimeOut());
            Thread.Sleep(100);

            Assert.AreEqual((leader.RoleState as LeaderState).BallotStatus, BallotStatus.Adopted);
            AcceptorTestUtil.StopAcceptors(acceptors);
        }

        [TestMethod]
        public void LeaderSolicitateBallotGetRejectedAndRetry()
        {
            List<Acceptor> acceptors = LeaderTestUtil.GenerateAcceptors(MessageBroker);
            List<MessageSender> acceptorsSenders = LeaderTestUtil.AcceptorInstanceListToMessageSenders(acceptors);
            List<MessageSender> replicas = LeaderTestUtil.GenerateFakeReplicas(null, MessageBroker);

            Leader leaderThatAcquireFirstBallot = LeaderTestUtil.GetAcceptorWithNoMocksRunning(acceptorsSenders, replicas, MessageBroker, new FakeTimeOut());
            Leader leaderThatGetDeclinedFirstBallot = LeaderTestUtil.GetAcceptorWithNoMocksRunning(acceptorsSenders, replicas, MessageBroker, new FakeTimeOut(), BallotNumber.Empty());

            Thread.Sleep(1000);

            LeaderState declineFirstBallotState = (leaderThatGetDeclinedFirstBallot.RoleState as LeaderState);
            LeaderState acquireFirstBallotState = (leaderThatAcquireFirstBallot.RoleState as LeaderState);

            Assert.AreEqual(declineFirstBallotState.BallotStatus, BallotStatus.Rejected);
            Assert.AreEqual(acquireFirstBallotState.BallotStatus, BallotStatus.Adopted);

            Assert.AreEqual(declineFirstBallotState.BallotNumber, acquireFirstBallotState.BallotNumber);

            FakeTimeOut fakeTimeOut = new FakeTimeOut();
            fakeTimeOut.Increase();
            fakeTimeOut.Wait();
            Assert.AreEqual(acquireFirstBallotState.BallotStatus, BallotStatus.Adopted);

            AcceptorTestUtil.StopAcceptors(acceptors);
        }
    }
}
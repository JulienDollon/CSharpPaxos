using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpPaxosRuntime.Roles;
using CSharpPaxosRuntime.Messaging;
using System.Threading.Tasks;
using System.Threading;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Roles.Acceptor;
using CSharpPaxosRuntime.Roles.RolesGeneric;
using CSharpPaxosRuntime.Utils.Log;

namespace CSharpPaxosRuntime.Tests
{
    [TestClass]
    public class ActorTest
    {
        public ActorTest()
        {
        }

        readonly IMessageBroker messageBroker = new ObjectsMessageBroker();

        [TestMethod]
        public void TestIfInstanceIsWellInitialized()
        {
            Acceptor acceptor = getAcceptorWithNoMocks();

            Assert.IsNotNull(acceptor.ActorState);
            Assert.IsNotNull(acceptor.ActorState.MessageSender);
            Assert.IsNotNull(acceptor.MessageReceiver);
        }

        [TestMethod]
        public void Test_IfLoopWorksWell()
        {
            Acceptor acceptor = getAcceptorWithNoMocks();

            Task t = Task.Run(() => {
                acceptor.Start();
            });

            Thread.Sleep(50);
            Assert.IsFalse(t.IsCompleted);

            acceptor.Stop();

            Thread.Sleep(50);
            Assert.IsTrue(t.IsCompleted);
        }

        [TestMethod]
        public void Test_WithValidBallot_VoteRequest()
        {
            int ballotNumber = 0;
            int slotNumber = 10;
            VoteStatus expectedVoteStatus = VoteStatus.Accepted;
            testVoteRequest(ballotNumber, slotNumber, expectedVoteStatus);
        }

        [TestMethod]
        public void Test_WithInValidBallot_VoteRequest()
        {
            int ballotNumber = -1;
            int slotNumber = 10;
            VoteStatus expectedVoteStatus = VoteStatus.Rejected;
            testVoteRequest(ballotNumber, slotNumber, expectedVoteStatus);
        }

        private VoteRequest generateVoteRequest(int ballotNumber, int slotNumber)
        {
            VoteRequest vote = new VoteRequest
            {
                BallotNumber = ballotNumber,
                MessageSender = new MessageSender() {UniqueId = this.GetHashCode().ToString()},
                SlotNumber = slotNumber
            };
            return vote;
        }

        [TestMethod]
        public void Test_WithValidBallot_SolicitateBallotRequest()
        {
            int ballotNumber = 1;
            testSolicitateBallotRequest(ballotNumber, ballotNumber);
        }

        [TestMethod]
        public void Test_WithInValidBallot_SolicitateBallotRequest()
        {
            int ballotNumber = -1;
            int defaultBallot = 0;
            testSolicitateBallotRequest(ballotNumber, defaultBallot);
        }

        private IMessage generateSolicitateBallotRequest(int ballotNumber)
        {
            SolicitateBallotRequest request = new SolicitateBallotRequest
            {
                MessageSender = new MessageSender() {UniqueId = this.GetHashCode().ToString()},
                BallotNumber = ballotNumber
            };
            return request;
        }

        private Acceptor getAcceptorWithNoMocks()
        {
            ILogger logger = new DebugModeLogger();
            IMessageReceiver receiver = new MessageReceiver();
            PaxosActorLoopMessageListener loopListener = new PaxosActorLoopMessageListener();

            return new Acceptor(logger, receiver, loopListener, messageBroker);
        }

        private void subscribeToBroker(MessageReceiver testReceiver)
        {
            this.messageBroker.AddReceiver(this.GetHashCode().ToString(), testReceiver);
        }

        private void cleanBrokerSubscription()
        {
            this.messageBroker.RemoveReceiver(this.GetHashCode().ToString());
        }

        private void testSolicitateBallotRequest(int ballotNumber, int expectedBallot)
        {
            Acceptor acceptor = getAcceptorWithNoMocks();
            MessageReceiver testReceiver = new MessageReceiver();
            subscribeToBroker(testReceiver);

            Task t = Task.Run(() => {
                acceptor.Start();
            });

            IMessage request = generateSolicitateBallotRequest(ballotNumber);
            messageBroker.SendMessage(acceptor.ActorState.MessageSender.UniqueId, request);
            Thread.Sleep(50);

            SolicitateBallotResponse response = testReceiver.GetLastMessage() as SolicitateBallotResponse;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.BallotNumber, expectedBallot);
            Assert.AreEqual(response.MessageSender.UniqueId, acceptor.ActorState.MessageSender.UniqueId);
            Assert.AreEqual((acceptor.ActorState as AcceptorState).BallotNumber, expectedBallot);

            acceptor.Stop();
            cleanBrokerSubscription();
        }

        private void testVoteRequest(int ballotNumber, int slotNumber, VoteStatus expectedVoteStatus)
        {
            Acceptor acceptor = getAcceptorWithNoMocks();
            MessageReceiver testReceiver = new MessageReceiver();
            subscribeToBroker(testReceiver);

            Task t = Task.Run(() => {
                acceptor.Start();
            });

            VoteRequest request = generateVoteRequest(ballotNumber, slotNumber);
            messageBroker.SendMessage(acceptor.ActorState.MessageSender.UniqueId, request);
            Thread.Sleep(50);

            VoteResponse response = testReceiver.GetLastMessage() as VoteResponse;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.VoteStatus, expectedVoteStatus);

            acceptor.Stop();
            cleanBrokerSubscription();
        }
    }
}
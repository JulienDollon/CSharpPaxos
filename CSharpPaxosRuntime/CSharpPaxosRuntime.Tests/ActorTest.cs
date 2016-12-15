using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpPaxosRuntime.Roles;
using CSharpPaxosRuntime.Log;
using CSharpPaxosRuntime.Messaging;
using System.Threading.Tasks;
using System.Threading;
using CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes;

namespace CSharpPaxosRuntime.Tests
{
    [TestClass]
    public class ActorTest
    {
        public ActorTest()
        {
        }

        IMessageBroker messageBroker = new ObjectsMessageBroker();

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
        public void Test_WithValidBallot_SolicitateBallotRequest()
        {
            int ballotNumber = 1;
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
            Assert.AreEqual(response.BallotNumber, ballotNumber);
            Assert.AreEqual(response.MessageSender.UniqueId, acceptor.ActorState.MessageSender.UniqueId);
            Assert.AreEqual((acceptor.ActorState as AcceptorState).BallotNumber, ballotNumber);

            acceptor.Stop();
            cleanBrokerSubscription();
        }

        [TestMethod]
        public void Test_WithUnValidBallot_SolicitateBallotRequest()
        {
            int ballotNumber = -1;
            int defaultBallot = 0;

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
            Assert.AreEqual(response.BallotNumber, defaultBallot);
            Assert.AreEqual(response.MessageSender.UniqueId, acceptor.ActorState.MessageSender.UniqueId);
            Assert.AreEqual((acceptor.ActorState as AcceptorState).BallotNumber, defaultBallot);

            acceptor.Stop();
            cleanBrokerSubscription();
        }

        private IMessage generateSolicitateBallotRequest(int ballotNumber)
        {
            SolicitateBallotRequest request = new SolicitateBallotRequest();
            request.MessageSender = new MessageSender() { UniqueId = this.GetHashCode().ToString() };
            request.BallotNumber = ballotNumber;
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
    }
}
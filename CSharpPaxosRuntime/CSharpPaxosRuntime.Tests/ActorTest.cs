using System.Threading;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Log;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Roles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpPaxosRuntime.Tests
{
    [TestClass]
    public class ActorTest
    {
        private readonly IMessageBroker _messageBroker = new ObjectsMessageBroker();

        [TestMethod]
        public void TestIfInstanceIsWellInitialized()
        {
            var acceptor = GetAcceptorWithNoMocks();
            Assert.IsNotNull(acceptor.ActorState);
            Assert.IsNotNull(acceptor.ActorState.MessageSender);
            Assert.IsNotNull(acceptor.MessageReceiver);
        }

        [TestMethod]
        public void Test_IfLoopWorksWell()
        {
            var acceptor = GetAcceptorWithNoMocks();

            var t = Task.Run(() => { acceptor.Start(); });

            Thread.Sleep(50);
            Assert.IsFalse(t.IsCompleted);

            acceptor.Stop();

            Thread.Sleep(50);
            Assert.IsTrue(t.IsCompleted);
        }

        [TestMethod]
        public void Test_WithValidBallot_VoteRequest()
        {
            var ballotNumber = 0;
            var slotNumber = 10;
            var expectedBallotNumber = 0;
            TestVoteRequest(ballotNumber, slotNumber, expectedBallotNumber);
        }

        [TestMethod]
        public void Test_WithInValidBallot_VoteRequest()
        {
            var ballotNumber = -1;
            var slotNumber = 10;
            var expectedBallotNumber = 0;
            TestVoteRequest(ballotNumber, slotNumber, expectedBallotNumber);
        }

        private VoteRequest GenerateVoteRequest(int ballotNumber, int slotNumber)
        {
            var vote = new VoteRequest
            {
                BallotNumber = ballotNumber,
                MessageSender = new MessageSender {UniqueId = GetHashCode().ToString()},
                SlotNumber = slotNumber
            };
            return vote;
        }

        [TestMethod]
        public void Test_WithValidBallot_SolicitateBallotRequest()
        {
            var ballotNumber = 1;
            TestSolicitateBallotRequest(ballotNumber, ballotNumber);
        }

        [TestMethod]
        public void Test_WithInValidBallot_SolicitateBallotRequest()
        {
            var ballotNumber = -1;
            var defaultBallot = 0;
            TestSolicitateBallotRequest(ballotNumber, defaultBallot);
        }

        private IMessage GenerateSolicitateBallotRequest(int ballotNumber)
        {
            var request = new SolicitateBallotRequest
            {
                MessageSender = new MessageSender {UniqueId = GetHashCode().ToString()},
                BallotNumber = ballotNumber
            };
            return request;
        }

        private Acceptor GetAcceptorWithNoMocks()
        {
            ILogger logger = new DebugModeLogger();
            IMessageReceiver receiver = new MessageReceiver();
            var loopListener = new PaxosActorLoopMessageListener();

            return new Acceptor(logger, receiver, loopListener, _messageBroker);
        }

        private void SubscribeToBroker(MessageReceiver testReceiver)
        {
            _messageBroker.AddReceiver(GetHashCode().ToString(), testReceiver);
        }

        private void CleanBrokerSubscription()
        {
            _messageBroker.RemoveReceiver(GetHashCode().ToString());
        }

        private void TestSolicitateBallotRequest(int ballotNumber, int expectedBallot)
        {
            var acceptor = GetAcceptorWithNoMocks();
            var testReceiver = new MessageReceiver();
            SubscribeToBroker(testReceiver);

            Task.Run(() => { acceptor.Start(); });

            var request = GenerateSolicitateBallotRequest(ballotNumber);
            _messageBroker.SendMessage(acceptor.ActorState.MessageSender.UniqueId, request);
            Thread.Sleep(50);

            var response = testReceiver.GetLastMessage() as SolicitateBallotResponse;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.BallotNumber, expectedBallot);
            Assert.AreEqual(response.MessageSender.UniqueId, acceptor.ActorState.MessageSender.UniqueId);
            Assert.AreEqual(((AcceptorState)acceptor.ActorState).BallotNumber, expectedBallot);

            acceptor.Stop();
            CleanBrokerSubscription();
        }

        private void TestVoteRequest(int ballotNumber, int slotNumber, int expectedBallotNumber)
        {
            var acceptor = GetAcceptorWithNoMocks();
            var testReceiver = new MessageReceiver();
            SubscribeToBroker(testReceiver);

            Task.Run(() => { acceptor.Start(); });

            var request = GenerateVoteRequest(ballotNumber, slotNumber);
            _messageBroker.SendMessage(acceptor.ActorState.MessageSender.UniqueId, request);
            Thread.Sleep(50);

            var response = testReceiver.GetLastMessage() as VoteResponse;

            Assert.IsNotNull(response);
            Assert.AreEqual(response.BallotNumber, expectedBallotNumber);

            acceptor.Stop();
            CleanBrokerSubscription();
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.Acceptor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpPaxosRuntime.Tests.Roles.AcceptorRole.IntegrationTests
{
    [TestClass]
    public class AcceptorTest
    {
        private readonly static IMessageBroker MessageBroker = new ObjectsMessageBroker();

        public AcceptorTest()
        {
            acceptorNotRunning = AcceptorTestUtil.GetAcceptorWithNoMocks(MessageBroker);
            acceptor = AcceptorTestUtil.GetAcceptorWithNoMocksRunning(MessageBroker);
        }

        private Acceptor acceptorNotRunning;
        private Acceptor acceptor;

        [TestMethod]
        public void AcceptorStartAndStop()
        {
            Task t = Task.Run(() => {
                acceptorNotRunning.Start();
            });

            Thread.Sleep(50);
            Assert.IsFalse(t.IsCompleted);

            acceptorNotRunning.Stop();

            Thread.Sleep(50);
            Assert.IsTrue(t.IsCompleted);
        }

        [TestMethod]
        public void AcceptorAcquireValidBallot()
        {
            BallotNumber number = BallotNumber.GenerateBallotNumber(10,10);
            tryAcquireBallot(number, number);
        }

        [TestMethod]
        public void AcceptorDeclineInValidBallot()
        {
            BallotNumber number = BallotNumber.GenerateBallotNumber(10, 10);
            tryAcquireBallot(number, number);

            BallotNumber invalidNumber = number.Decrement();
            tryAcquireBallot(invalidNumber, number);
        }

        [TestMethod]
        public void AcceptorVoteWithValidBallot()
        {
            BallotNumber number = BallotNumber.GenerateBallotNumber(10, 10);
            tryAcquireBallot(number, number);
            VoteRequest request = AcceptorTestUtil.GenerateVoteRequest(number);
            VoteResponse response = AcceptorTestUtil.SendVoteRequest(acceptor, request, number, MessageBroker);

            Assert.AreEqual(response.VoteStatus, VoteStatus.Accepted);
            Assert.IsNotNull((acceptor.RoleState as AcceptorState).AcceptedDecision);
        }

        [TestMethod]
        public Acceptor AcceptorVoteWithInValidBallot()
        {
            BallotNumber number = BallotNumber.GenerateBallotNumber(10, 10);
            tryAcquireBallot(number, number);

            BallotNumber invalidNumber = number.Decrement();
            VoteRequest request = AcceptorTestUtil.GenerateVoteRequest(invalidNumber);
            VoteResponse response = AcceptorTestUtil.SendVoteRequest(acceptor, request, number, MessageBroker);

            Assert.AreEqual(response.VoteStatus, VoteStatus.Rejected);
            return acceptor;
        }

        private void tryAcquireBallot(BallotNumber ballotNumber, BallotNumber expectBallotNumber)
        {
            SolicitateBallotResponse response = AcceptorTestUtil.SendSolicitateBallotRequest(acceptor, ballotNumber, MessageBroker);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.BallotNumber, expectBallotNumber);
            Assert.AreEqual(response.MessageSender.UniqueId, acceptor.RoleState.MessageSender.UniqueId);
            Assert.AreEqual((acceptor.RoleState as AcceptorState).BallotNumber, expectBallotNumber);
        }
    }
}
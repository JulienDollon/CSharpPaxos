using System.Threading;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.Acceptor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpPaxosRuntime.Tests.Roles.AcceptorRole.IntegrationTests
{
    [TestClass]
    public class AcceptorTest
    {
        private readonly
            Acceptor acceptorNotRunning = AcceptorTestUtil.GetAcceptorWithNoMocks();
        private readonly
            Acceptor acceptor = AcceptorTestUtil.GetAcceptorWithNoMocksRunning();

        [TestMethod]
        public void StartAndStop()
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
        public void AcquireValidBallot()
        {
            BallotNumber number = BallotNumber.GenerateBallotNumber(10,10);
            tryAcquireBallot(number, number);
        }

        [TestMethod]
        public void DeclineInValidBallot()
        {
            BallotNumber number = BallotNumber.GenerateBallotNumber(10, 10);
            tryAcquireBallot(number, number);

            BallotNumber invalidNumber = new BallotNumber() { Value = number.Value - 1 };
            tryAcquireBallot(invalidNumber, number);
        }

        [TestMethod]
        public void VoteWithValidBallot()
        {
            BallotNumber number = BallotNumber.GenerateBallotNumber(10, 10);
            tryAcquireBallot(number, number);
            VoteRequest request = AcceptorTestUtil.GenerateVoteRequest(number);
            VoteResponse response = AcceptorTestUtil.SendVoteRequest(acceptor, request, number);

            Assert.AreEqual(response.VoteStatus, VoteStatus.Accepted);
            Assert.IsNotNull((acceptor.RoleState as AcceptorState).AcceptedDecision);
        }

        [TestMethod]
        public Acceptor VoteWithInValidBallot()
        {
            BallotNumber number = BallotNumber.GenerateBallotNumber(10, 10);
            tryAcquireBallot(number, number);

            BallotNumber invalidNumber = new BallotNumber() { Value = number.Value - 1 };
            VoteRequest request = AcceptorTestUtil.GenerateVoteRequest(invalidNumber);
            VoteResponse response = AcceptorTestUtil.SendVoteRequest(acceptor, request, number);

            Assert.AreEqual(response.VoteStatus, VoteStatus.Rejected);
            return acceptor;
        }

        private void tryAcquireBallot(BallotNumber ballotNumber, BallotNumber expectBallotNumber)
        {
            SolicitateBallotResponse response = AcceptorTestUtil.SendSolicitateBallotRequest(acceptor, ballotNumber);

            Assert.IsNotNull(response);
            Assert.AreEqual(response.BallotNumber, expectBallotNumber);
            Assert.AreEqual(response.MessageSender.UniqueId, acceptor.RoleState.MessageSender.UniqueId);
            Assert.AreEqual((acceptor.RoleState as AcceptorState).BallotNumber, expectBallotNumber);
        }
    }
}
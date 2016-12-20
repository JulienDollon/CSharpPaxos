using System.Threading;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.Acceptor;
using CSharpPaxosRuntime.Roles.RolesGeneric;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpPaxosRuntime.Tests.Roles.AcceptorRole.IntegrationTests
{
    public static class AcceptorTestUtil
    {
        public static readonly IMessageBroker MessageBroker = new ObjectsMessageBroker();

        public static Acceptor GetAcceptorWithNoMocks()
        {
            IMessageReceiver receiver = new MessageReceiver();

            PaxosRoleLoopMessageListener loopListener = new PaxosRoleLoopMessageListener();
            Acceptor acceptor = new Acceptor(receiver, loopListener, MessageBroker);

            Assert.IsNotNull(acceptor.RoleState);
            Assert.IsNotNull(acceptor.RoleState.MessageSender);
            Assert.IsNotNull(acceptor.MessageReceiver);
            return acceptor;
        }

        public static Acceptor GetAcceptorWithNoMocksRunning()
        {
            Acceptor acceptor = AcceptorTestUtil.GetAcceptorWithNoMocks();

            Task t = Task.Run(() => {
                acceptor.Start();
            });

            return acceptor;
        }

        public static VoteRequest GenerateVoteRequest(BallotNumber ballotNumber)
        {
            VoteRequest vote = new VoteRequest
            {
                BallotNumber = ballotNumber,
                MessageSender = new MessageSender() { UniqueId = "FAKE" },
            };
            return vote;
        }

        public static IMessage GenerateSolicitateBallotRequest(BallotNumber ballotNumber)
        {
            SolicitateBallotRequest request = new SolicitateBallotRequest
            {
                MessageSender = new MessageSender() { UniqueId = "FAKE" },
                BallotNumber = ballotNumber
            };
            return request;
        }

        public static void SubscribeToBroker(MessageReceiver testReceiver)
        {
            MessageBroker.AddReceiver("FAKE", testReceiver);
        }

        public static void CleanBrokerSubscription()
        {
            MessageBroker.RemoveReceiver("FAKE");
        }

        public static SolicitateBallotResponse SendSolicitateBallotRequest(Acceptor acceptor, BallotNumber ballotNumber)
        {
            IMessage request = AcceptorTestUtil.GenerateSolicitateBallotRequest(ballotNumber);
            MessageReceiver testReceiver = new MessageReceiver();
            SubscribeToBroker(testReceiver);

            MessageBroker.SendMessage(acceptor.RoleState.MessageSender.UniqueId, request);
            Thread.Sleep(50);

            SolicitateBallotResponse response = testReceiver.GetLastMessage() as SolicitateBallotResponse;

            return response;
        }

        public static VoteResponse SendVoteRequest(Acceptor acceptor, VoteRequest request, BallotNumber ballotNumber)
        {
            MessageReceiver testReceiver = new MessageReceiver();
            SubscribeToBroker(testReceiver);

            MessageBroker.SendMessage(acceptor.RoleState.MessageSender.UniqueId, request);
            Thread.Sleep(50);

            VoteResponse response = testReceiver.GetLastMessage() as VoteResponse;
            return response;
        }
    }
}
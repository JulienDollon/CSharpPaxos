using System.Collections.Generic;
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
        public static Acceptor GetAcceptorWithNoMocks(IMessageBroker broker)
        {
            IMessageReceiver receiver = new MessageReceiver();

            PaxosRoleLoopMessageListener loopListener = new PaxosRoleLoopMessageListener();
            Acceptor acceptor = new Acceptor(receiver, loopListener, broker);

            Assert.IsNotNull(acceptor.RoleState);
            Assert.IsNotNull(acceptor.RoleState.MessageSender);
            Assert.IsNotNull(acceptor.MessageReceiver);
            return acceptor;
        }

        public static Acceptor GetAcceptorWithNoMocksRunning(IMessageBroker broker)
        {
            Acceptor acceptor = AcceptorTestUtil.GetAcceptorWithNoMocks(broker);

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

        public static void SubscribeToBroker(MessageReceiver testReceiver, IMessageBroker broker)
        {
            broker.AddReceiver("FAKE", testReceiver);
        }

        public static void CleanBrokerSubscription(IMessageBroker broker)
        {
            broker.RemoveReceiver("FAKE");
        }

        public static SolicitateBallotResponse SendSolicitateBallotRequest(Acceptor acceptor, BallotNumber ballotNumber, IMessageBroker broker)
        {
            IMessage request = AcceptorTestUtil.GenerateSolicitateBallotRequest(ballotNumber);
            MessageReceiver testReceiver = new MessageReceiver();
            SubscribeToBroker(testReceiver, broker);

            broker.SendMessage(acceptor.RoleState.MessageSender.UniqueId, request);
            Thread.Sleep(50);

            SolicitateBallotResponse response = testReceiver.GetLastMessage() as SolicitateBallotResponse;

            return response;
        }

        public static VoteResponse SendVoteRequest(Acceptor acceptor, VoteRequest request, BallotNumber ballotNumber, IMessageBroker broker)
        {
            MessageReceiver testReceiver = new MessageReceiver();
            SubscribeToBroker(testReceiver, broker);

            broker.SendMessage(acceptor.RoleState.MessageSender.UniqueId, request);
            Thread.Sleep(50);

            VoteResponse response = testReceiver.GetLastMessage() as VoteResponse;
            return response;
        }

        public static void StopAcceptors(List<Acceptor> acceptors)
        {
            foreach (Acceptor acceptor in acceptors)
            {
                acceptor.Stop();
            }
            Thread.Sleep(10);
        }
    }
}
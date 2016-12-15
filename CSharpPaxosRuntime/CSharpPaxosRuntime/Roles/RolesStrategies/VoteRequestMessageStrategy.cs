using System;
using System.Diagnostics;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Roles.RolesStrategies
{
    public class VoteRequestMessageStrategy : IMessageStrategy
    {
        private readonly IMessageBroker _broker;

        public VoteRequestMessageStrategy(IMessageBroker broker)
        {
            _broker = broker;
        }

        public void Execute(MessageStrategyExecuteArg<IMessage> obj)
        {
            CheckInvalidParameter(obj);
            var request = obj.Message as VoteRequest;
            var state = obj.ActorState as AcceptorState;

            Debug.Assert(request != null, "request != null");
            Debug.Assert(state != null, "state != null");
            if (BallotIsValid(request.BallotNumber, state.BallotNumber))
                AcceptVote(request, state);
            else
                RejectVote();
            var response = GenerateVoteResponse(state, request);
            SendVoteResponse(request.MessageSender, response);
        }

        private void SendVoteResponse(MessageSender sendTo, VoteResponse response)
        {
            _broker.SendMessage(sendTo.UniqueId, response);
        }

        private VoteResponse GenerateVoteResponse(AcceptorState state, VoteRequest request)
        {
            var response = new VoteResponse
            {
                BallotNumber = state.BallotNumber,
                MessageSender = state.MessageSender,
                SlotNumber = request.SlotNumber
            };
            return response;
        }

        private void RejectVote()
        {
            // Method intentionally left empty.
        }

        private void AcceptVote(VoteRequest request, AcceptorState state)
        {
            var vote = new VoteDecision
            {
                BallotNumber = request.BallotNumber,
                SlotNumber = request.SlotNumber,
                Command = request.Command
            };
            state.AcceptedDecisions.Add(vote);
        }

        private bool BallotIsValid(int ballotNumber1, int ballotNumber2)
        {
            return ballotNumber1 == ballotNumber2;
        }

        private void CheckInvalidParameter(MessageStrategyExecuteArg<IMessage> obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (!(obj.Message is VoteRequest))
                throw new MessageStrategyException("This strategy shouldn't be invoked with this message type");

            if (!(obj.ActorState is AcceptorState))
                throw new MessageStrategyException("This strategy should only be executed by an acceptor");
        }
    }
}
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.RolesGeneric;

namespace CSharpPaxosRuntime.Roles.Acceptor.AcceptorStrategies
{
    public class SendVoteStrategy : IMessageStrategy
    {
        private readonly IMessageBroker broker;
        public SendVoteStrategy(IMessageBroker broker)
        {
            this.broker = broker;
        }

        public void Execute(MessageStrategyExecuteArg<IMessage> obj)
        {
            checkInvalidParameter(obj);
            VoteRequest request = (obj.Message as VoteRequest);
            AcceptorState state = obj.RoleState as AcceptorState;

            if (ballotIsValid(request.BallotNumber, state.BallotNumber))
            {
                acceptVote(request, state);
            }
            else
            {
                rejectVote(request, state);
            }
        }

        private void sendVoteResponse(MessageSender sendTo, VoteResponse response)
        {
            this.broker.SendMessage(sendTo.UniqueId, response);
        }

        private VoteResponse generateVoteResponse(AcceptorState state, VoteRequest request, VoteStatus voteStatus)
        {
            VoteResponse response = new VoteResponse
            {
                BallotNumber = state.BallotNumber,
                MessageSender = state.MessageSender,
                SlotNumber = request.SlotNumber,
                VoteStatus = voteStatus
            };
            return response;
        }

        private void rejectVote(VoteRequest request, AcceptorState state)
        {
            VoteStatus voteStatus = VoteStatus.Rejected;
            VoteResponse response = generateVoteResponse(state, request, voteStatus);
            sendVoteResponse(request.MessageSender, response);
        }

        private void acceptVote(VoteRequest request, AcceptorState state)
        {
            VoteDecision vote = new VoteDecision
            {
                BallotNumber = request.BallotNumber,
                SlotNumber = request.SlotNumber,
                Command = request.Command
            };
            state.AcceptedDecision = vote;

            VoteStatus voteStatus = VoteStatus.Accepted;
            VoteResponse response = generateVoteResponse(state, request, voteStatus);
            sendVoteResponse(request.MessageSender, response);
        }

        private bool ballotIsValid(BallotNumber ballotNumber1, BallotNumber ballotNumber2)
        {
            return ballotNumber1 == ballotNumber2;
        }

        private void checkInvalidParameter(MessageStrategyExecuteArg<IMessage> obj)
        {
            if (!(obj.Message is VoteRequest))
            {
                throw new MessageStrategyException("This strategy shouldn't be invoked with this message type");
            }

            if (!(obj.RoleState is AcceptorState))
            {
                throw new MessageStrategyException("This strategy should only be executed by an acceptor");
            }
        }
    }
}
using CSharpPaxosRuntime.RolesStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Roles.RolesStrategies
{
    public class VoteRequestMessageStrategy : IMessageStrategy
    {
        private IMessageBroker broker;
        public VoteRequestMessageStrategy(IMessageBroker broker)
        {
            this.broker = broker;
        }

        public void Execute(MessageStrategyExecuteArg<IMessage> obj)
        {
            checkInvalidParameter(obj);
            VoteRequest request = (obj.Message as VoteRequest);
            AcceptorState state = obj.ActorState as AcceptorState;

            if (this.ballotIsValid(request.BallotNumber, state.BallotNumber))
            {
                acceptVote(request, state);
            }
            else
            {
                rejectVote();
            }

            VoteResponse response = generateVoteResponse(state, request);
            sendVoteResponse(request.MessageSender, response);
        }

        private void sendVoteResponse(MessageSender sendTo, VoteResponse response)
        {
            this.broker.SendMessage(sendTo.UniqueId, response);
        }

        private VoteResponse generateVoteResponse(AcceptorState state, VoteRequest request)
        {
            VoteResponse response = new VoteResponse();
            response.BallotNumber = state.BallotNumber;
            response.MessageSender = state.MessageSender;
            response.SlotNumber = request.SlotNumber;
            return response;
        }

        private void rejectVote() { }

        private void acceptVote(VoteRequest request, AcceptorState state)
        {
            VoteDecision vote = new VoteDecision();
            vote.BallotNumber = request.BallotNumber;
            vote.SlotNumber = request.SlotNumber;
            vote.Command = request.Command;
            state.AcceptedDecisions.Add(vote);
        }

        private bool ballotIsValid(int ballotNumber1, int ballotNumber2)
        {
            return ballotNumber1 == ballotNumber2;
        }

        private void checkInvalidParameter(MessageStrategyExecuteArg<IMessage> obj)
        {
            if (!(obj.Message is VoteRequest))
            {
                throw new MessageStrategyException("This strategy shouldn't be invoked with this message type");
            }

            if (!(obj.ActorState is AcceptorState))
            {
                throw new MessageStrategyException("This strategy should only be executed by an acceptor");
            }
        }
    }
}
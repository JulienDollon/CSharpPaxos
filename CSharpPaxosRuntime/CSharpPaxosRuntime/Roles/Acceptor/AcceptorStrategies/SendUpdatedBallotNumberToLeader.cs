using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.RolesGeneric;

namespace CSharpPaxosRuntime.Roles.Acceptor.AcceptorStrategies
{
    public class SendUpdatedBallotNumberToLeader : IMessageStrategy
    {
        private readonly IMessageBroker broker;
        public SendUpdatedBallotNumberToLeader(IMessageBroker broker)
        {
            this.broker = broker;
        }

        public void Execute(MessageStrategyExecuteArg<IMessage> obj)
        {
            checkInvalidParameter(obj);

            SolicitateBallotRequest message = obj.Message as SolicitateBallotRequest;
            AcceptorState state = obj.RoleState as AcceptorState;

            updateBallotNumberIfNeeded(message, state);
            sendSolicitateBallotResponse(message.MessageSender, state);
        }

        private void sendSolicitateBallotResponse(MessageSender sendTo, AcceptorState state)
        {
            SolicitateBallotResponse response = new SolicitateBallotResponse
            {
                BallotNumber = state.BallotNumber,
                MessageSender = state.MessageSender,
                Decision = state.LastAcceptedVote
            };
            this.broker.SendMessage(sendTo.UniqueId, response);
        }

        private void updateBallotNumberIfNeeded(SolicitateBallotRequest message, AcceptorState state)
        {
            if (message.BallotNumber > state.BallotNumber)
            {
                state.BallotNumber = message.BallotNumber;
            }
        }

        private void checkInvalidParameter(MessageStrategyExecuteArg<IMessage> obj)
        {
            if (!(obj.Message is SolicitateBallotRequest))
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
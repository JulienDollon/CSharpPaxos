using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Roles.RolesGeneric;

namespace CSharpPaxosRuntime.Roles.Acceptor.AcceptorStrategies
{
    public class SolicitateBallotRequestMessageStrategy : IMessageStrategy
    {
        private readonly IMessageBroker broker;
        public SolicitateBallotRequestMessageStrategy(IMessageBroker broker)
        {
            this.broker = broker;
        }

        public void Execute(MessageStrategyExecuteArg<IMessage> obj)
        {
            checkInvalidParameter(obj);

            SolicitateBallotRequest message = obj.Message as SolicitateBallotRequest;
            AcceptorState state = obj.ActorState as AcceptorState;

            updateBallotNumberIfNeeded(message, state);
            sendSolicitateBallotResponse(message.MessageSender, state);
        }

        private void sendSolicitateBallotResponse(MessageSender sendTo, AcceptorState state)
        {
            SolicitateBallotResponse response = new SolicitateBallotResponse
            {
                BallotNumber = state.BallotNumber,
                MessageSender = state.MessageSender,
                Decisions = state.AcceptedDecisions
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

            if (!(obj.ActorState is AcceptorState))
            {
                throw new MessageStrategyException("This strategy should only be executed by an acceptor");
            }
        }
    }
}
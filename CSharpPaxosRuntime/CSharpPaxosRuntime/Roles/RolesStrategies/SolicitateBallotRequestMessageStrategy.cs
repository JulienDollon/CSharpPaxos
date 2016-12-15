using System;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.Properties;

namespace CSharpPaxosRuntime.Roles.RolesStrategies
{
    public class SolicitateBallotRequestMessageStrategy : IMessageStrategy
    {
        private readonly IMessageBroker _broker;
        public SolicitateBallotRequestMessageStrategy(IMessageBroker broker)
        {
            _broker = broker;
        }

        public void Execute(MessageStrategyExecuteArg<IMessage> obj)
        {
            CheckInvalidParameter(obj);

            SolicitateBallotRequest message = obj.Message as SolicitateBallotRequest;
            AcceptorState state = obj.ActorState as AcceptorState;

            UpdateBallotNumberIfNeeded(message, state);
            SendSolicitateBallotResponse(message?.MessageSender, state);
        }

        private void SendSolicitateBallotResponse(MessageSender sendTo, AcceptorState state)
        {
            SolicitateBallotResponse response = new SolicitateBallotResponse
            {
                BallotNumber = state.BallotNumber,
                MessageSender = state.MessageSender,
                Decisions = state.AcceptedDecisions
            };
            _broker.SendMessage(sendTo.UniqueId, response);
        }

        private void UpdateBallotNumberIfNeeded(SolicitateBallotRequest message, AcceptorState state)
        {
            if (message.BallotNumber > state.BallotNumber)
            {
                state.BallotNumber = message.BallotNumber;
            }
        }

        private void CheckInvalidParameter(MessageStrategyExecuteArg<IMessage> obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
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
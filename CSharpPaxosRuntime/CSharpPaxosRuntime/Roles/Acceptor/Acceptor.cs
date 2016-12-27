using System;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.Acceptor.AcceptorStrategies;
using CSharpPaxosRuntime.Roles.RolesGeneric;
namespace CSharpPaxosRuntime.Roles.Acceptor
{
    public class Acceptor : IPaxosRole
    {
        private readonly IPaxosRoleLoopMessageListener loopListener;
        private AcceptorState currentAcceptorState;
        private StrategyContainer strategyContainer;

        public Acceptor(IMessageReceiver receiver, 
                        IPaxosRoleLoopMessageListener loopListener,
                        IMessageBroker messageBroker)
        {
            this.MessageReceiver = receiver;
            this.loopListener = loopListener;
            this.MessageBroker = messageBroker;

            this.initializeState();
            this.initializeLoopListener();
            this.defineSupportedMessage();
        }

        private void initializeLoopListener()
        {
            this.loopListener.Initialize(this.MessageReceiver,
                                         this.onMessageDequeued,
                                         this.MessageBroker,
                                         this.currentAcceptorState.MessageSender);
        }

        private void initializeState()
        {
            this.currentAcceptorState = new AcceptorState
            {
                MessageSender = new MessageSender()
                {
                    UniqueId = this.GetHashCode().ToString()
                },
                BallotNumber = BallotNumber.Empty()
            };

        }

        private void defineSupportedMessage()
        {
            this.strategyContainer = new StrategyContainer();
            this.strategyContainer.AddStrategy(typeof(SolicitateBallotRequest), 
                new SendUpdatedBallotNumberToLeader(this.MessageBroker));

            this.strategyContainer.AddStrategy(typeof(VoteRequest),
                new SendVoteResponseToLeader(this.MessageBroker));
        }

        public void Start()
        {
            loopListener.Execute();
        }

        public void Stop()
        {
            loopListener.KeepRunning = false;
        }

        private void onMessageDequeued(IMessage lastMessage)
        {
            this.strategyContainer.ExecuteStrategy(lastMessage, this.currentAcceptorState);
        }

        public IPaxosRoleState RoleState => currentAcceptorState;
        public IMessageReceiver MessageReceiver { get; set; }
        public IMessageBroker MessageBroker { get; }
    }
}
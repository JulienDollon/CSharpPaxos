using System;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Roles.Acceptor.AcceptorStrategies;
using CSharpPaxosRuntime.Roles.RolesGeneric;
using CSharpPaxosRuntime.Utils.Log;

namespace CSharpPaxosRuntime.Roles.Acceptor
{
    public class Acceptor : IPaxosActor
    {
        private ILogger logger;
        private readonly IPaxosActorLoopMessageListener loopListener;
        private AcceptorState currentAcceptorState;
        private StrategyContainer strategyContainer;

        public Acceptor(ILogger logger, 
                        IMessageReceiver receiver, 
                        IPaxosActorLoopMessageListener loopListener,
                        IMessageBroker messageBroker)
        {
            this.logger = logger;
            this.MessageReceiver = receiver;
            this.loopListener = loopListener;
            this.MessageBroker = messageBroker;

            this.initializeState();
            this.initializeLoopListener();
            this.defineSupportedMessageTypes();
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

        private void defineSupportedMessageTypes()
        {
            this.strategyContainer = new StrategyContainer();
            this.strategyContainer.AddStrategy(typeof(SolicitateBallotRequest), 
                new SolicitateBallotRequestMessageStrategy(this.MessageBroker));

            this.strategyContainer.AddStrategy(typeof(VoteRequest),
                new VoteRequestMessageStrategy(this.MessageBroker));
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

        public IPaxosActorState ActorState => currentAcceptorState;
        public IMessageReceiver MessageReceiver { get; set; }

        public IMessageBroker MessageBroker { get; }
    }
}
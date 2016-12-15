using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Log;
using CSharpPaxosRuntime.Utils;
using CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.RolesStrategies;
using CSharpPaxosRuntime.Roles.RolesStrategies;

namespace CSharpPaxosRuntime.Roles
{
    public class Acceptor : IPaxosActor
    {
        const int defaultBallotNumber = 0;

        private ILogger logger;
        private IMessageReceiver messageReceiver;
        private IMessageBroker messageBroker;
        private IPaxosActorLoopMessageListener loopListener;
        private AcceptorState currentAcceptorState;
        private StrategyContainer strategyContainer;

        public Acceptor(ILogger logger, 
                        IMessageReceiver receiver, 
                        IPaxosActorLoopMessageListener loopListener,
                        IMessageBroker messageBroker)
        {
            this.logger = logger;
            this.messageReceiver = receiver;
            this.loopListener = loopListener;
            this.messageBroker = messageBroker;

            this.initializeState();
            this.initializeLoopListener();
            this.initializeMessageStrategies();
        }

        private void initializeLoopListener()
        {
            this.loopListener.Initialize(this.MessageReceiver,
                                         this.onMessageDequeued,
                                         this.messageBroker,
                                         this.currentAcceptorState.MessageSender);
        }

        private void initializeState()
        {
            this.currentAcceptorState = new AcceptorState();
            this.currentAcceptorState.MessageSender = new MessageSender()
                {
                    UniqueId = this.GetHashCode().ToString()
                };

            this.currentAcceptorState.BallotNumber = defaultBallotNumber;
        }

        private void initializeMessageStrategies()
        {
            this.strategyContainer = new StrategyContainer();
            this.strategyContainer.AddStrategy(typeof(SolicitateBallotRequest), 
                new SolicitateBallotRequestMessageStrategy(this.messageBroker));
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

        public IMessageReceiver MessageReceiver
        {
            get
            {
                return this.messageReceiver;
            }
        }

        public IPaxosActorState ActorState
        {
            get
            {
                return currentAcceptorState;
            }
        }
    }
}
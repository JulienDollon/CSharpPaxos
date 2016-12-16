using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Roles.RolesGeneric;
using CSharpPaxosRuntime.Roles.RolesStrategies;
using CSharpPaxosRuntime.Utils.Log;

namespace CSharpPaxosRuntime.Roles.Acceptor
{
    public class Acceptor : IPaxosActor
    {
        const int defaultBallotNumber = 0;

        private ILogger logger;
        private readonly IMessageBroker messageBroker;
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
            this.currentAcceptorState = new AcceptorState
            {
                MessageSender = new MessageSender()
                {
                    UniqueId = this.GetHashCode().ToString()
                },
                BallotNumber = defaultBallotNumber
            };

        }

        private void initializeMessageStrategies()
        {
            this.strategyContainer = new StrategyContainer();
            this.strategyContainer.AddStrategy(typeof(SolicitateBallotRequest), 
                new SolicitateBallotRequestMessageStrategy(this.messageBroker));

            this.strategyContainer.AddStrategy(typeof(VoteRequest),
                new VoteRequestMessageStrategy(this.messageBroker));
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

        public IMessageReceiver MessageReceiver { get; }

        public IPaxosActorState ActorState => currentAcceptorState;
    }
}
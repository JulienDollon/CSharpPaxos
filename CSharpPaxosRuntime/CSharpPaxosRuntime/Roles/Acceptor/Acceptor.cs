using CSharpPaxosRuntime.Log;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Messaging.Properties;
using CSharpPaxosRuntime.Roles.RolesStrategies;

namespace CSharpPaxosRuntime.Roles
{
    public class Acceptor : IPaxosActor
    {
        private const int defaultBallotNumber = 0;

        private readonly ILogger _logger;
        private readonly IPaxosActorLoopMessageListener _loopListener;
        private readonly IMessageBroker _messageBroker;
        private AcceptorState _currentAcceptorState;
        private StrategyContainer _strategyContainer;

        public Acceptor(ILogger logger,
            IMessageReceiver receiver,
            IPaxosActorLoopMessageListener loopListener,
            IMessageBroker messageBroker)
        {
            _logger = logger;
            MessageReceiver = receiver;
            _loopListener = loopListener;
            _messageBroker = messageBroker;

            InitializeState();
            InitializeLoopListener();
            InitializeMessageStrategies();
        }

        public void Start()
        {
            _loopListener.Execute();
        }

        public void Stop()
        {
            _loopListener.KeepRunning = false;
        }

        public IMessageReceiver MessageReceiver { get; }

        public IPaxosActorState ActorState => _currentAcceptorState;

        private void InitializeLoopListener()
        {
            _loopListener.Initialize(MessageReceiver,
                OnMessageDequeued,
                _messageBroker,
                _currentAcceptorState.MessageSender);
        }

        private void InitializeState()
        {
            _currentAcceptorState = new AcceptorState
            {
                MessageSender = new MessageSender
                {
                    UniqueId = GetHashCode().ToString()
                },
                BallotNumber = defaultBallotNumber
            };
        }

        private void InitializeMessageStrategies()
        {
            _strategyContainer = new StrategyContainer();
            _strategyContainer.AddStrategy(typeof(SolicitateBallotRequest),
                new SolicitateBallotRequestMessageStrategy(_messageBroker));

            _strategyContainer.AddStrategy(typeof(VoteRequest),
                new VoteRequestMessageStrategy(_messageBroker));
        }

        private void OnMessageDequeued(IMessage lastMessage)
        {
            _strategyContainer.ExecuteStrategy(lastMessage, _currentAcceptorState);
        }
    }
}
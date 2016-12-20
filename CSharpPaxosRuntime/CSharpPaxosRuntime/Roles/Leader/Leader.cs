using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.Leader.LeaderStrategies;
using CSharpPaxosRuntime.Roles.RolesGeneric;

namespace CSharpPaxosRuntime.Roles.Leader
{
    public class Leader : IPaxosRole
    {
        private readonly int defaultRound = 0;
        private readonly IPaxosRoleLoopMessageListener loopListener;
        private StrategyContainer strategyContainer;

        public Leader(IMessageReceiver receiver,
                IPaxosRoleLoopMessageListener loopListener,
                IMessageBroker messageBroker,
                List<MessageSender> acceptors)
        {
            this.MessageReceiver = receiver;
            this.loopListener = loopListener;
            this.MessageBroker = messageBroker;

            this.initializeState(acceptors);
            this.initializeLoopListener();
            this.defineSupportedMessage();
        }

        private void defineSupportedMessage()
        {
            this.strategyContainer = new StrategyContainer();
            this.strategyContainer.AddStrategy(typeof(SolicitateBallotRequest),
                new RequestBallotStrategy(this.MessageBroker));

            ReceiveUpdatedBallotNumberStrategy receiveUpdatedBallot = new ReceiveUpdatedBallotNumberStrategy();
            receiveUpdatedBallot.OnBallotAdopted += receiveUpdatedBallot_OnBallotAdopted;
            receiveUpdatedBallot.OnBallotRejected += ReceiveUpdatedBallot_OnBallotRejected;

            this.strategyContainer.AddStrategy(typeof(SolicitateBallotResponse), receiveUpdatedBallot);
        }

        private void ReceiveUpdatedBallot_OnBallotRejected(object sender, EventArgs e)
        {
            //TODO
        }

        private void receiveUpdatedBallot_OnBallotAdopted(object sender, EventArgs e)
        {
            //TODO
        }

        private void initializeLoopListener()
        {
            this.loopListener.Initialize(this.MessageReceiver,
                 this.onMessageDequeued,
                 this.MessageBroker,
                 this.RoleState.MessageSender);
        }

        private void initializeState(List<MessageSender> acceptors)
        {
            int uniqueId = this.GetHashCode();
            this.currentState = new LeaderState
            {
                MessageSender = new MessageSender()
                {
                    UniqueId = uniqueId.ToString()
                },
                BallotStatus = BallotStatus.Rejected,
                BallotNumber = BallotNumber.GenerateBallotNumber(defaultRound, uniqueId),
                Acceptors = acceptors
            };
        }

        public IMessageReceiver MessageReceiver { get; set; }
        public IMessageBroker MessageBroker { get; }
        public IPaxosRoleState RoleState => currentState;
        private LeaderState currentState;

        public void Start()
        {
            solicitateBallot();
            loopListener.Execute();
        }

        private void solicitateBallot()
        {
            SolicitateBallotRequest request = new SolicitateBallotRequest();
            request.BallotNumber = this.currentState.BallotNumber;
            request.MessageSender = this.currentState.MessageSender;
            this.strategyContainer.ExecuteStrategy(request, this.currentState);
        }

        public void Stop()
        {
            loopListener.KeepRunning = false;
        }

        private void onMessageDequeued(IMessage lastMessage)
        {
            this.strategyContainer.ExecuteStrategy(lastMessage, this.RoleState);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Environment;
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
        private readonly ITimeOut timeToWaitBetweenOperations;

        public Leader(IMessageReceiver receiver,
                IPaxosRoleLoopMessageListener loopListener,
                IMessageBroker messageBroker,
                List<MessageSender> acceptors,
                List<MessageSender> replicas,
                ITimeOut timeToWaitBetweenOperations)
        {
            this.MessageReceiver = receiver;
            this.loopListener = loopListener;
            this.MessageBroker = messageBroker;
            this.timeToWaitBetweenOperations = timeToWaitBetweenOperations;

            this.initializeState(acceptors, replicas);
            this.initializeLoopListener();
            this.defineSupportedMessage();
        }

        private void defineSupportedMessage()
        {
            this.strategyContainer = new StrategyContainer();
            this.strategyContainer.AddStrategy(typeof(SolicitateBallotRequest),
                new SendSolicitateBallotRequestStrategy(this.MessageBroker));

            ReceiveUpdatedBallotNumberStrategy receiveUpdatedBallot = new ReceiveUpdatedBallotNumberStrategy();
            receiveUpdatedBallot.BallotRejected += onBallotRejected;
            receiveUpdatedBallot.BallotApproved += onBallotApproved;
            this.strategyContainer.AddStrategy(typeof(SolicitateBallotResponse), receiveUpdatedBallot);

            this.strategyContainer.AddStrategy(typeof(VoteRequest),
                new SendVoteRequestStrategy(this.MessageBroker));

            ReceiveVoteResponseStrategy receiveVote = new ReceiveVoteResponseStrategy();
            receiveVote.OnApprovalPreempted += onBallotRejected;
            receiveVote.OnApprovalElected += onApprovalElected;
            this.strategyContainer.AddStrategy(typeof(VoteResponse), receiveVote);
        }

        private void onApprovalElected(object sender, IMessage message)
        {
            cleanProposals();
            notifyAllReplicasOfElectedValue(message);
        }

        private void notifyAllReplicasOfElectedValue(IMessage message)
        {
            foreach (MessageSender replica in this.currentState.Replicas)
            {
                this.MessageBroker.SendMessage(replica.UniqueId, message);
            }
        }

        private void onBallotApproved(object sender, EventArgs e)
        {
            reduceLatency();
            findPendingProposalsPropagatedByOtherLeaders();
            sendCurrentAndPendingProposals();
            cleanProposals();
        }

        private void cleanProposals()
        {
            this.currentState.ProposalsBySlotId.Clear();
        }

        private void sendCurrentAndPendingProposals()
        {
            foreach (KeyValuePair<int, ICommand> proposal in this.currentState.ProposalsBySlotId)
            {
                VoteRequest request = new VoteRequest();
                request.BallotNumber = this.currentState.BallotNumber;
                request.Command = proposal.Value;
                request.SlotNumber = proposal.Key;
                request.MessageSender = this.currentState.MessageSender;
                this.strategyContainer.ExecuteStrategy(request, this.currentState);
            }
        }

        private void findPendingProposalsPropagatedByOtherLeaders()
        {
            Dictionary<int, BallotNumber> highestBallotNumbersPerSlot = new Dictionary<int, BallotNumber>();
            foreach (VoteDecision voteDecision in this.currentState.ValuesAcceptedByAcceptors)
            {
                if (!highestBallotNumbersPerSlot.ContainsKey(voteDecision.SlotNumber) ||
                    highestBallotNumbersPerSlot[voteDecision.SlotNumber] < voteDecision.BallotNumber)
                {
                    highestBallotNumbersPerSlot[voteDecision.SlotNumber] = voteDecision.BallotNumber;
                    this.currentState.ProposalsBySlotId.Add(voteDecision.SlotNumber, voteDecision.Command);
                }
            }
        }

        private void reduceLatency()
        {
            this.timeToWaitBetweenOperations.ResetToDefault();
        }

        private void onBallotRejected(object sender, EventArgs eventArgs)
        {
            currentState.BallotStatus = BallotStatus.Rejected;
            exponentionalBackoff();
            solicitateBallot();
        }

        private void exponentionalBackoff()
        {
            this.timeToWaitBetweenOperations.Increase();
            this.timeToWaitBetweenOperations.Wait();
        }

        private void initializeLoopListener()
        {
            this.loopListener.Initialize(this.MessageReceiver,
                 this.onMessageDequeued,
                 this.MessageBroker,
                 this.RoleState.MessageSender);
        }

        private void initializeState(List<MessageSender> acceptors, List<MessageSender> replicas)
        {
            int uniqueId = this.GetHashCode();
            this.currentState = new LeaderState
            {
                MessageSender = new MessageSender()
                {
                    UniqueId = uniqueId.ToString()
                },
                BallotStatus = BallotStatus.Undetermined,
                BallotNumber = BallotNumber.GenerateBallotNumber(defaultRound, uniqueId),
                Acceptors = acceptors,
                Replicas = replicas
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
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
using log4net;

namespace CSharpPaxosRuntime.Roles.Leader
{
    public class Leader : IPaxosRole
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Leader));
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
                new SendSolicitateBallotRequestToAcceptors(this.MessageBroker));

            ReceiveUpdatedBallotNumberFromAcceptors receiveUpdatedBallot = new ReceiveUpdatedBallotNumberFromAcceptors();
            receiveUpdatedBallot.BallotRejected += onBallotRejected;
            receiveUpdatedBallot.BallotApproved += onBallotApproved;
            this.strategyContainer.AddStrategy(typeof(SolicitateBallotResponse), receiveUpdatedBallot);

            this.strategyContainer.AddStrategy(typeof(VoteRequest),
                new SendVoteRequestToAcceptors(this.MessageBroker));

            ReceiveVoteResponseFromAcceptors receiveVote = new ReceiveVoteResponseFromAcceptors();
            receiveVote.OnApprovalPreempted += onBallotRejected;
            receiveVote.OnApprovalElected += onProposalElected;
            this.strategyContainer.AddStrategy(typeof(VoteResponse), receiveVote);

            ReceiveProposalRequestFromReplica requestFromReplica = new ReceiveProposalRequestFromReplica();
            requestFromReplica.OnProposalReceived += onProposalFromReplicaReceived;
            this.strategyContainer.AddStrategy(typeof(ProposalRequest), requestFromReplica);
        }

        private void onProposalFromReplicaReceived(object sender, EventArgs eventArgs)
        {
            if (this.currentState.BallotStatus == BallotStatus.Adopted)
            {
                logger.Info($"{this.currentState.MessageSender.UniqueId} received proposal from replica and is organizing an election");
                organizeVote();
            }
        }

        private void onProposalElected(object sender, IMessage message)
        {
            notifyAllReplicasOfElectedValue(message as VoteResponse);
        }

        private void notifyAllReplicasOfElectedValue(VoteResponse message)
        {
            ProposalDecision decision = new ProposalDecision(message);
            decision.Command = this.currentState.ProposalsBySlotId[message.SlotNumber];
            foreach (MessageSender replica in this.currentState.Replicas)
            {
                this.MessageBroker.SendMessage(replica.UniqueId, decision);
            }
        }

        private void onBallotApproved(object sender, EventArgs e)
        {
            organizeVote();
        }

        private void organizeVote()
        {
            reduceLatency();
            updateInMemoryProposals();
            sendPendingProposalsToAcceptors();
        }

        private void sendPendingProposalsToAcceptors()
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

        private void updateInMemoryProposals()
        {
            List<VoteResponse> pendingProposalsPropagatedByOtherLeaders = this.currentState.ValuesAcceptedByAcceptors;
            Dictionary<int, BallotNumber> highestBallotNumberPerSlot = new Dictionary<int, BallotNumber>();

            foreach (VoteResponse voteDecision in pendingProposalsPropagatedByOtherLeaders)
            {
                if (!highestBallotNumberPerSlot.ContainsKey(voteDecision.SlotNumber) ||
                    highestBallotNumberPerSlot[voteDecision.SlotNumber] < voteDecision.BallotNumber)
                {
                    highestBallotNumberPerSlot[voteDecision.SlotNumber] = voteDecision.BallotNumber;
                    updateInMemoryProposal(voteDecision.SlotNumber, voteDecision.Command);
                }
            }
        }

        private void updateInMemoryProposal(int slotNumber, ICommand command)
        {
            if (!this.currentState.ProposalsBySlotId.ContainsKey(slotNumber))
            {
                this.currentState.ProposalsBySlotId.Add(slotNumber, command);
            }
            else
            {
                this.currentState.ProposalsBySlotId[slotNumber] = command;
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
            logger.Info($"{this.currentState.MessageSender.UniqueId} is starting");
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Messaging.Bus;
using CSharpPaxosRuntime.Models;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;
using CSharpPaxosRuntime.Roles.Acceptor;
using CSharpPaxosRuntime.Roles.Acceptor.AcceptorStrategies;
using CSharpPaxosRuntime.Roles.Replica.ReplicaStrategies;
using CSharpPaxosRuntime.Roles.RolesGeneric;

namespace CSharpPaxosRuntime.Roles.Replica
{
    public class Replica : IPaxosRole
    {
        private readonly IPaxosRoleLoopMessageListener loopListener;
        private ReplicaState currentReplicaState;
        private StrategyContainer strategyContainer;

        public Replica(IMessageReceiver receiver,
                       IPaxosRoleLoopMessageListener loopListener,
                       IMessageBroker messageBroker,
                       IStateMachine stateMachine)
        {
            this.MessageReceiver = receiver;
            this.loopListener = loopListener;
            this.MessageBroker = messageBroker;

            this.initializeState(stateMachine);
            this.initializeLoopListener();
            this.defineSupportedMessage();
        }

        private void initializeLoopListener()
        {
            this.loopListener.Initialize(this.MessageReceiver,
                                         this.onMessageDequeued,
                                         this.MessageBroker,
                                         this.currentReplicaState.MessageSender);
        }

        private void initializeState(IStateMachine stateMachine)
        {
            this.currentReplicaState = new ReplicaState()
            {
                MessageSender = new MessageSender()
                {
                    UniqueId = this.GetHashCode().ToString()
                },
                StateMachine = stateMachine
            };
        }

        private void defineSupportedMessage()
        {
            this.strategyContainer = new StrategyContainer();
            this.strategyContainer.AddStrategy(typeof(ClientRequest),
                new SendProposalRequestToLeaders(this.MessageBroker));
            ReceiveProposalDecisionFromLeader receiveProposalDecision = new ReceiveProposalDecisionFromLeader();
            receiveProposalDecision.OnDecisionApproved += onDecisionApproved;
            receiveProposalDecision.OnDecisionRejected += onDecisionRejected;
            this.strategyContainer.AddStrategy(typeof(ProposalDecision), receiveProposalDecision);
        }

        private void onDecisionApproved(object sender, ClientRequest decision)
        {
            applyDecisionsToStateMachine();
        }

        private void applyDecisionsToStateMachine()
        {
            int minimumDecisionToApply = this.currentReplicaState.LastAppliedSlotToStateMachine;
            for (int i = minimumDecisionToApply; i < this.currentReplicaState.DecisionsBySlotId.Count - minimumDecisionToApply; i++)
            {
                if (this.currentReplicaState.DecisionsBySlotId.ContainsKey(i))
                {
                    this.currentReplicaState.LastAppliedSlotToStateMachine++;
                    this.currentReplicaState.StateMachine.Update(this.currentReplicaState.DecisionsBySlotId[i]);
                    sendDecisionToClient(this.currentReplicaState.ClientsPendingResponseBySlotId[i], VoteStatus.Accepted);
                }
            }
        }

        private void onDecisionRejected(object sender, ClientRequest request)
        {
            retryToSendProposalToLeaders(request);
        }

        private void retryToSendProposalToLeaders(ClientRequest request)
        {
            this.strategyContainer.ExecuteStrategy(request, this.currentReplicaState);
        }

        private void sendDecisionToClient(ClientRequest request, VoteStatus status)
        {
            ClientResponse response = new ClientResponse();
            response.VoteStatus = status;
            response.MessageSender = this.currentReplicaState.MessageSender;
            response.Command = request.Command;

            this.MessageBroker.SendMessage(request.MessageSender.UniqueId, response);
        }

        public void Start()
        {
            if (this.currentReplicaState.Leaders.Count == 0)
            {
                throw new InvalidOperationException("You need to setup leaders in the replica's state before starting replicas");
            }
            loopListener.Execute();
        }

        public void Stop()
        {
            loopListener.KeepRunning = false;
        }

        private void onMessageDequeued(IMessage lastMessage)
        {
            this.strategyContainer.ExecuteStrategy(lastMessage, this.currentReplicaState);
        }

        public IPaxosRoleState RoleState => currentReplicaState;
        public IMessageReceiver MessageReceiver { get; set; }
        public IMessageBroker MessageBroker { get; }
    }
}
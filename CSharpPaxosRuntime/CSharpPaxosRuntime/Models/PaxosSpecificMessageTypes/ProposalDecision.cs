using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Messaging.Bus;

namespace CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes
{
    public class ProposalDecision : IDecisionMessage
    {
        public ProposalDecision(IDecisionMessage message)
        {
            this.BallotNumber = message.BallotNumber;
            this.MessageSender = message.MessageSender;
            this.SlotNumber = message.SlotNumber;
            this.VoteStatus = message.VoteStatus;
            this.Command = message.Command;
        }

        public BallotNumber BallotNumber
        {
            get;
            set;
        }

        public MessageSender MessageSender
        {
            get;
            set;
        }

        public int SlotNumber
        {
            get;
            set;
        }

        public VoteStatus VoteStatus
        {
            get;
            set;
        }

        public ICommand Command
        {
            get;
            set;
        }
    }
}
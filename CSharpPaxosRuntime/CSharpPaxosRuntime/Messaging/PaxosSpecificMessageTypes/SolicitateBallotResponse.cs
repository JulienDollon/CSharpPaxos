using CSharpPaxosRuntime.Messaging.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.Messaging.PaxosSpecificMessageTypes
{
    public class SolicitateBallotResponse : IMessage, IBallotNumberProperty, IDecisionsProperty
    {
        public int BallotNumber
        {
            get;
            set;
        }

        public VoteDecisions Decisions
        {
            get;
            set;
        }

        public MessageSender MessageSender
        {
            get;
            set;
        }
    }
}
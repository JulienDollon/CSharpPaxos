using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Models.PaxosSpecificMessageTypes;

namespace CSharpPaxosRuntime.Models
{
    public class Decision : IDecision
    {
        public BallotNumber BallotNumber { get; set; }
        public int SlotNumber { get; set; }
        public VoteStatus VoteStatus { get; set; }
        public ICommand Command { get; set; }
    }
}
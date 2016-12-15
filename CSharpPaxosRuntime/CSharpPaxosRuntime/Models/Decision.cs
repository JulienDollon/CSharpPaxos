using CSharpPaxosRuntime.Messaging.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Models
{
    public class Decision : ISlotNumberProperty, IBallotNumberProperty, ICommandProperty
    {
        public int BallotNumber { get; set; }

        public ICommand Command { get; set; }

        public int SlotNumber { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Messaging.Properties
{
    public interface IBallotNumberProperty
    {
        int BallotNumber { get; set; }
    }
}
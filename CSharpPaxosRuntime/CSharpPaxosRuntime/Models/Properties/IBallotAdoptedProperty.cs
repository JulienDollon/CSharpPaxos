using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Models.Properties
{
    public interface IBallotAdoptedProperty
    {
        BallotStatus BallotStatus { get; set; }
    }
}
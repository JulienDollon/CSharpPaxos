using CSharpPaxosRuntime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Messaging.Properties
{
    public interface IDecisionsProperty
    {
        VoteDecisions Decisions { get; set; }
    }
}
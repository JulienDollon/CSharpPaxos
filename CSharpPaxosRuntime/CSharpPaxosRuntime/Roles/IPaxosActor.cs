using CSharpPaxosRuntime.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Roles
{
    internal interface IPaxosActor
    {
        IMessageReceiver Receiver { get; }
    }
}
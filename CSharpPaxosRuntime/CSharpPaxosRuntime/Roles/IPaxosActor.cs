using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.RolesStrategies;
using CSharpPaxosRuntime.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Roles
{
    internal interface IPaxosActor
    {
        IMessageReceiver MessageReceiver { get; }
        IPaxosActorState ActorState { get; }
        void Start();
        void Stop();
    }
}
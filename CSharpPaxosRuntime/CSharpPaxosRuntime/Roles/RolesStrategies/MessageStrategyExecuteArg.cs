using CSharpPaxosRuntime.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Roles.RolesStrategies
{
    public class MessageStrategyExecuteArg<T> where T : IMessage
    {
        public T Message { get; set; }
        public IPaxosActorState ActorState { get; set; }
    }
}
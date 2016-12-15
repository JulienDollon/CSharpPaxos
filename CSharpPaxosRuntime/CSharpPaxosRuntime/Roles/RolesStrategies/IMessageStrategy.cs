using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Roles;
using CSharpPaxosRuntime.Roles.RolesStrategies;
using CSharpPaxosRuntime.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.RolesStrategies
{
    public interface IMessageStrategy : IStrategy<MessageStrategyExecuteArg<IMessage>>
    {
    }
}
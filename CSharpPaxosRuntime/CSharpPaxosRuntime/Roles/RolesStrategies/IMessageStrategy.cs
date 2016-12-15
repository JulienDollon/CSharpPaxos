using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Utils;

namespace CSharpPaxosRuntime.Roles.RolesStrategies
{
    public interface IMessageStrategy : IStrategy<MessageStrategyExecuteArg<IMessage>>
    {
    }
}
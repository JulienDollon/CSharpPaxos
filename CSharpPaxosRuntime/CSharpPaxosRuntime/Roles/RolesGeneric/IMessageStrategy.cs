using CSharpPaxosRuntime.Messaging;
using CSharpPaxosRuntime.Utils;

namespace CSharpPaxosRuntime.Roles.RolesGeneric
{
    public interface IMessageStrategy : IStrategy<MessageStrategyExecuteArg<IMessage>>
    {
    }
}
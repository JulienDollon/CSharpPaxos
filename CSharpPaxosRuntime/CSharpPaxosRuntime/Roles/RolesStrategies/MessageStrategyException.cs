using System;

namespace CSharpPaxosRuntime.Roles.RolesStrategies
{
    class MessageStrategyException : InvalidOperationException
    {
        public MessageStrategyException(string msg) : base(msg)
        {
        }
    }
}
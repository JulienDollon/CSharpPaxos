using System;

namespace CSharpPaxosRuntime.Roles.RolesStrategies
{
    internal class MessageStrategyException : InvalidOperationException
    {
        public MessageStrategyException(string msg) : base(msg)
        {
        }
    }
}
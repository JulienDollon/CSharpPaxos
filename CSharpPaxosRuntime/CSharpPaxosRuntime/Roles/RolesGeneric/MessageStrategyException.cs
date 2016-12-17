using System;

namespace CSharpPaxosRuntime.Roles.RolesGeneric
{
    class MessageStrategyException : InvalidOperationException
    {
        public MessageStrategyException(string msg) : base(msg)
        {
        }
    }
}
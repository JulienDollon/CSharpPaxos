using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Roles.RolesStrategies
{
    class MessageStrategyException : InvalidOperationException
    {
        public MessageStrategyException(string msg) : base(msg)
        {
        }
    }
}
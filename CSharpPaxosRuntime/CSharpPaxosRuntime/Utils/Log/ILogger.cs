using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Log
{
    public interface ILogger
    {
        void Log(Severity severity, string message);
    }
}
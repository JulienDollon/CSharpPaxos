using System;

namespace CSharpPaxosRuntime.Log
{
    public class DebugModeLogger : ILogger
    {
        public void Log(Severity severity, string message)
        {
            Console.WriteLine($"Log:{severity}, {message}");
        }
    }
}
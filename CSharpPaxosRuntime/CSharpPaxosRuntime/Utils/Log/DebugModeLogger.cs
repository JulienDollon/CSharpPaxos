using System;

namespace CSharpPaxosRuntime.Log
{
    public class DebugModeLogger : ILogger
    {
        public void Log(Severity severity, string message)
        {
            Console.WriteLine(string.Format("Log:{0}, {1}, {2}", severity.ToString(), message));
        }
    }
}
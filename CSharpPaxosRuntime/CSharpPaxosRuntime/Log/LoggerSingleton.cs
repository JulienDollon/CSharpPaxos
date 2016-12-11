using System;

namespace CSharpPaxosRuntime.Log
{
    public class LoggerSingleton
    {
        private static ILogger instance;

        private LoggerSingleton() { }

        public static ILogger Instance
        {
            get
            {
                if (instance == null)
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        instance = new DebugModeLogger();
                    }
                    else
                    {
                        throw new NotImplementedException("No logger for production code yet");
                    }
                }
                return instance;
            }
        }
    }
}
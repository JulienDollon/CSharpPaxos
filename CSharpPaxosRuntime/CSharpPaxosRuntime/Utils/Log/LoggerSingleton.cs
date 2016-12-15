using System;

namespace CSharpPaxosRuntime.Log
{
    public class LoggerSingleton
    {
        private static ILogger _instance;

        private LoggerSingleton() { }

        public static ILogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        _instance = new DebugModeLogger();
                    }
                    else
                    {
                        throw new NotImplementedException("No logger for production code yet");
                    }
                }
                return _instance;
            }
        }
    }
}
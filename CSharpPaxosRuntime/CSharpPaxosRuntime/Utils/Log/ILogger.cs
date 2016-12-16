namespace CSharpPaxosRuntime.Utils.Log
{
    public interface ILogger
    {
        void Log(Severity severity, string message);
    }
}
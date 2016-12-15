namespace CSharpPaxosRuntime.Utils
{
    public interface IStrategy
    {
        void Execute();
    }

    public interface IStrategy<in T>
    {
        void Execute(T obj);
    }
}
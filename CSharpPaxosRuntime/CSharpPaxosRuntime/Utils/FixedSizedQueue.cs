using System.Collections.Concurrent;
using CSharpPaxosRuntime.Utils.Log;

namespace CSharpPaxosRuntime.Utils
{
    public class FixedSizedQueue<T>
    {
        readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
        private readonly int max_size;
        public FixedSizedQueue(int max_size = 5000000)
        {
            this.max_size = max_size;
        }

        public T Dequeue()
        {
            T outObj;
            queue.TryDequeue(out outObj);
            return outObj;
        }

        public void Enqueue(T obj)
        {
            this.queue.Enqueue(obj);

            while (this.queue.Count > this.max_size)
            {
                T outObj;
                queue.TryDequeue(out outObj);
                LoggerSingleton.Instance.Log(Severity.Info, "Messages lost, max size of the queue reached");
            }
        }
    }
}
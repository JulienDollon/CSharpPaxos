using System.Collections.Concurrent;
using CSharpPaxosRuntime.Log;

namespace CSharpPaxosRuntime.Utils
{
    public class FixedSizedQueue<T>
    {
        private readonly int _maxSize;
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public FixedSizedQueue(int maxSize = 5000000)
        {
            _maxSize = maxSize;
        }

        public T Dequeue()
        {
            T outObj;
            _queue.TryDequeue(out outObj);
            return outObj;
        }

        public void Enqueue(T obj)
        {
            _queue.Enqueue(obj);

            while (_queue.Count > _maxSize)
            {
                T outObj;
                _queue.TryDequeue(out outObj);
                LoggerSingleton.Instance.Log(Severity.Info, "Messages lost, max size of the queue reached");
            }
        }
    }
}
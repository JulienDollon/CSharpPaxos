using CSharpPaxosRuntime.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Utils
{
    public class FixedSizedQueue<T>
    {
        private readonly object privateLockObject = new object();
        readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
        private int max_size;
        public FixedSizedQueue(int max_size = 5000000)
        {
            this.max_size = max_size;
        }

        public T Dequeue()
        {
            lock (this.privateLockObject)
            {
                T outObj;
                queue.TryDequeue(out outObj);
                return outObj;
            }
        }

        public void Enqueue(T obj)
        {
            this.queue.Enqueue(obj);
            lock (this.privateLockObject)
            {
                while (this.queue.Count > this.max_size)
                {
                    T outObj;
                    queue.TryDequeue(out outObj);
                    LoggerSingleton.Instance.Log(Severity.Info, "Messages lost, max size of the queue reached");
                }
            }
        }
    }
}
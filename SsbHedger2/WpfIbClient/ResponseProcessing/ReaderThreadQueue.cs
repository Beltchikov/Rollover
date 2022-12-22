using System.Collections.Concurrent;
using System.Linq;

namespace SsbHedger2.ResponseProcessing
{
    public class ReaderThreadQueue : IReaderThreadQueue
    {
        private readonly ConcurrentQueue<object> _queue;

        public ReaderThreadQueue()
        {
            _queue = new ConcurrentQueue<object>();
        }

        public void Enqueue(object item)
        {
            _queue.Enqueue(item);
        }

        public object? Dequeue()
        {
            object? item;
            if (_queue.TryDequeue(out item))
            {
                return item;
            }
            else
            {
                return null;
            }
        }

        public int Count()
        {
            return _queue.Count();
        }


    }
}

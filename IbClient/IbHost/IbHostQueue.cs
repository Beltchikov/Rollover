using System.Collections.Concurrent;
using System.Linq;

namespace StockAnalyzer.Ib
{
    public class IbHostQueue : IIbHostQueue
    {
        private readonly ConcurrentQueue<object> _ibClientQueue;

        public IbHostQueue()
        {
            _ibClientQueue = new ConcurrentQueue<object>();
        }

        public void Enqueue(object item)
        {
            _ibClientQueue.Enqueue(item);
        }

        public object Dequeue()
        {
            object item;
            if (_ibClientQueue.TryDequeue(out item))
            {
                return item;
            }
            else
            {
                return null;
            }
        }

        public object Peek()
        {
            object item;
            if (_ibClientQueue.TryPeek(out item))
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
            return _ibClientQueue.Count();
        }
    }
}

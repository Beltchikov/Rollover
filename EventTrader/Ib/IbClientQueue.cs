using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Eomn.Ib
{
    [ExcludeFromCodeCoverage(Justification = "Just a wrapper")]
    public class IbClientQueue : IIbClientQueue
    {
        private readonly ConcurrentQueue<object> _ibClientQueue;

        public IbClientQueue()
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

        public int Count()
        {
            return _ibClientQueue.Count();
        }
    }
}

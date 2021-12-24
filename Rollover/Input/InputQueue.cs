using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rollover.Input
{
    [ExcludeFromCodeCoverage(Justification ="Just a wrapper")]
    public class InputQueue : IInputQueue
    {
        private readonly ConcurrentQueue<string> _inputQueue;

        public InputQueue()
        {
            _inputQueue = new ConcurrentQueue<string>();
        }
        
        public void Enqueue(string item)
        {
            _inputQueue.Enqueue(item);
        }

        string IInputQueue.Dequeue()
        {
            string item;
            if(_inputQueue.TryDequeue(out item))
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
            return _inputQueue.Count(); 
        }
    }
}

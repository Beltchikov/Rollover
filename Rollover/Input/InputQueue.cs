using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollover.Input
{
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

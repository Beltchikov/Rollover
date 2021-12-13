using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollover.Input
{
    public class InputQueue : IInputQueue
    {
        private readonly Queue<string> _inputQueue;

        public InputQueue()
        {
            _inputQueue = new Queue<string>();
        }
        
        public void Enqueue(string item)
        {
            _inputQueue.Enqueue(item);
        }

        string IInputQueue.Dequeue()
        {
            return _inputQueue.Any() 
                ? _inputQueue.Dequeue()
                : null;
        }

        public int Count()
        { 
            return _inputQueue.Count(); 
        }
    }
}

using IbClient.messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IbClient.IbHost
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

        public bool DequeueMessage<T>(out T message)
        {
            object lockObject = new object();

            lock (lockObject)
            {
                if (!HasMessageInQueue<T>())
                {
                    message = default(T);
                    return false;
                }
                message = (T)Dequeue();
                return true;
            }
        }

        public bool DequeueMessage<T>(int reqId, out T message)
        {
            object lockObject = new object();

            lock (lockObject)
            {
                if (!HasMessageInQueue<T>(reqId))
                {
                    message = default(T);
                    return false;
                }
                message = (T)Dequeue();
                return true;
            }
        }

        private bool HasMessageInQueue<T>()
        {
            var message = Peek();
            if (message == null)
            {
                return false;
            }

            if (!(message is T))
            {
                return false;
            }

            return true;
        }

        // TODO make generic
        public void DequeueAllTickPriceMessageExcept(int reqId)
        {
            var itemsToKeep = new List<object>();
            object item = null;

            while (_ibClientQueue.TryDequeue(out item))
            {
                if(item is TickPriceMessage)
                {
                    var itemTyped = item as TickPriceMessage;
                    if(itemTyped.RequestId == reqId)
                    {
                        itemsToKeep.Add(item);
                    }
                }
            }

            foreach(var itemToAdd in itemsToKeep)
            {
                _ibClientQueue.Enqueue(itemToAdd);
            }
        }

        //TODO make private
        public bool HasMessageInQueue<T>(int reqId)
        {
            var message = Peek();
            if (message == null)
            {
                return false;
            }

            if (!(message is T))
            {
                return false;
            }

            if (message is ErrorMessage)
            {
                var errorMessage = message as ErrorMessage;
                return errorMessage?.RequestId == reqId;
            }
            if (message is ContractDetailsMessage)
            {
                var contractDetailsMessage = message as ContractDetailsMessage;
                return contractDetailsMessage?.RequestId == reqId;
            }

            if (message is TickPriceMessage)
            {
                var tickPriceMessage = message as TickPriceMessage;
                return tickPriceMessage?.RequestId == reqId;
            }

            if (message is OpenOrderMessage)
            {
                var openOrderMessage = message as OpenOrderMessage;
                return openOrderMessage?.OrderId == reqId;
            }

            if (message is SymbolSamplesMessage)
            {
                var symbolSamplesMessage = message as SymbolSamplesMessage;
                return symbolSamplesMessage?.ReqId == reqId;
            }

            throw new NotImplementedException();
        }

        public void Clear()
        {
            while (_ibClientQueue.TryDequeue(out _)){}
        }
    }
}

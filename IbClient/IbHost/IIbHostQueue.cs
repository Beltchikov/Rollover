﻿using System;

namespace IbClient.IbHost
{
    public interface IIbHostQueue
    {
        void Enqueue(object obj);
        object Dequeue();
        object Peek();
        int Count();
        bool DequeueMessage<T>(out T message);
        bool DequeueMessage<T>(int reqId, out T message);

        // TODO remove later
        bool HasMessageInQueue<T>(int reqId);
        void DequeueAllTickPriceMessagesExcept(int reqId);
        void DequeueAllOpenOrderMessagesExcept(int nextOrderId);
        void Clear();
        bool TryDequeue(out object item);
    }
}
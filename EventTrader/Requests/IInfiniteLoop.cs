using System;

namespace EventTrader.Requests
{
    public interface IInfiniteLoop
    {
        void Start(Action action, object[] parameters);
    }
}
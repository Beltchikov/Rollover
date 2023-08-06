using System;

namespace EventTrader.Requests
{
    public interface IInfiniteLoop
    {
        void Start(Action action, object[] parameters);
        
        event Action<string> Status;
        bool Stopped { get; set; }
        bool IsRunning { get; set; }


    }
}
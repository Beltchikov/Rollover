using System;
using System.Threading.Tasks;

namespace EventTrader.Requests
{
    public interface IInfiniteLoop
    {
        Task Start(Action action, object[] parameters);
        
        event Action<string> Status;
        bool Stopped { get; set; }
        bool IsRunning { get; set; }


    }
}
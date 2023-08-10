using System.Threading.Tasks;
using System;

namespace EventTrader
{
    public interface IEconDataLoop
    {
        Task StartAsync(int frequency, string dataType);
        event Action<string> Status;
        bool Stopped { get; set; }
        bool IsRunning { get; set; }
    }
}
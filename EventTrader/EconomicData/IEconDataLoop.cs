using System.Threading.Tasks;
using System;

namespace EventTrader
{
    public interface IEconDataLoop
    {
        Task StartAsync(
            int frequency,
            string dataType,
            string url,
            string xPathActual,
            string xPathExpected,
            string xPathPrevious,
            string nullPlaceholder);
        event Action<string> Status;
        bool Stopped { get; set; }
        bool IsRunning { get; set; }
    }
}
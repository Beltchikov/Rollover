using EventTrader.Requests;
using System;
using System.Threading.Tasks;

namespace EventTrader.EconomicData
{
    public class EconDataLoop : IEconDataLoop
    {
        private IInfiniteLoop _requestLoop;

        public EconDataLoop(IInfiniteLoop requestLoop)
        {
            _requestLoop = requestLoop;
            _requestLoop.Status += _requestLoop_Status;
        }
        public bool Stopped { get => _requestLoop.Stopped; set => _requestLoop.Stopped = value; }
        public bool IsRunning { get => _requestLoop.IsRunning; set => _requestLoop.IsRunning = value; }

        public event Action<string> Status = null!;

        public async Task StartAsync(int frequency, string dataType)
        {
            // TODO Strategy pattern IEconomicDataProvider
            await _requestLoop.StartAsync(() => { }, new object[] { frequency});
        }
        private void _requestLoop_Status(string obj)
        {
            Status.Invoke(obj); 
        }
    }
}

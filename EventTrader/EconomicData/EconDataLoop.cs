using EventTrader.Requests;
using System;
using System.Threading.Tasks;

namespace EventTrader.EconomicData
{
    public class EconDataLoop : IEconDataLoop
    {
        private IInfiniteLoop _requestLoop;
        private IDataProviderContext _dataProviderContext;

        public EconDataLoop(IInfiniteLoop requestLoop, IDataProviderContext dataProviderContext)
        {
            _requestLoop = requestLoop;
            _requestLoop.Status += _requestLoop_Status;
            _dataProviderContext = dataProviderContext;
        }
        public bool Stopped { get => _requestLoop.Stopped; set => _requestLoop.Stopped = value; }
        public bool IsRunning { get => _requestLoop.IsRunning; set => _requestLoop.IsRunning = value; }

        public event Action<string> Status = null!;

        public async Task StartAsync(int frequency, string dataType)
        {
            _dataProviderContext.SetStrategy(dataType);
            await _requestLoop.StartAsync(() => { return _dataProviderContext.GetData(); }, new object[] { frequency});
        }
        private void _requestLoop_Status(string obj)
        {
            Status.Invoke(obj); 
        }
    }
}

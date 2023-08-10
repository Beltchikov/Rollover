using System;
using System.Threading.Tasks;

namespace EventTrader.EconomicData
{
    public class EconDataLoop : IEconDataLoop
    {
        public bool Stopped { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsRunning { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event Action<string> Status;

        public Task StartAsync(int frequency, string dataType)
        {
            throw new NotImplementedException();
        }
    }
}

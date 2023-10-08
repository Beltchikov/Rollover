using System;

namespace StockAnalyzer.DataProviders
{
    public class ProviderBase
    {
        public event Action<string> Status = null!;

        protected void TriggerStatus(string message)
        {
            Status?.Invoke(message);
        }
    }
}

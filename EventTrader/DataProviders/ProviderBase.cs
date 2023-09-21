using System;

namespace Dsmn.DataProviders
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

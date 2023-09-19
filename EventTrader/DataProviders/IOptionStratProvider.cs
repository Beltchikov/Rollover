using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dsmn.DataProviders
{
    public interface IOptionStratProvider
    {
        public event Action<string> Status;
        Task<List<string>> HasCriticalWarningsAsync(List<string> tickerList, int delay);
    }
}
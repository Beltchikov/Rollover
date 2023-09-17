using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dsmn.DataProviders
{
    public interface IYahooProvider
    {
        public event Action<string> Status;
        Task<List<string>> ExpectedEpsAsync(List<string> tickerList);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public interface IYahooProvider
    {
        public event Action<string> Status;
        Task<List<string>> ExpectedEpsAsync(List<string> tickerList, int delay);
        Task<List<string>> LastEpsAsync(List<string> tickerList, int delay);
    }
}
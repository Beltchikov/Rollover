using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public interface ISeekingAlphaProvider
    {           
        Task<IEnumerable<string>> PeersComparison(string tickerList, int delay);
    }
}
using System.Collections.Generic;

namespace StockAnalyzer.DataProviders
{
    public interface ISeekingAlphaProvider
    {           
        IEnumerable<string> PeersComparison(List<string> tickerList, int delay);
    }
}
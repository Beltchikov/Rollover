using System.Collections.Generic;

namespace StockAnalyzer.DataProviders
{
    public interface IInvestingProvider
    {
        List<string> GetEarningsData(string htmlSource, double minMarketCap);
    }
}
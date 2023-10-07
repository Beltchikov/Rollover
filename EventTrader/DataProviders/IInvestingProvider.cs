using System.Collections.Generic;

namespace Eomn.DataProviders
{
    public interface IInvestingProvider
    {
        List<string> GetEarningsData(string htmlSource, double minMarketCap);
    }
}
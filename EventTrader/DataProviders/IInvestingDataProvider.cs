using System.Collections.Generic;

namespace Dsmn.DataProviders
{
    public interface IInvestingDataProvider
    {
        List<string> ExpectedEps(List<string> tickerList);
    }
}
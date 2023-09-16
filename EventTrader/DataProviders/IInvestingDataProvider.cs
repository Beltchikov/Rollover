using System.Collections.Generic;

namespace Dsmn.DataProviders
{
    public interface IInvestingDataProvider
    {
        List<string> ExpectedEps(string urlEpsExpected, string xPathEpsExpected, string nullPlaceholderEpsExpected, List<string> tickerList);
    }
}
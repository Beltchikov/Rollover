using System.Collections.Generic;

namespace Dsmn.DataProviders
{
    public interface IYahooProvider
    {
        List<string> ExpectedEps(List<string> tickerList);
    }
}
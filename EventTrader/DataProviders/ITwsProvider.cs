using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eomn.DataProviders
{
    public interface ITwsProvider
    {
        Task<List<string>> GetContractDetails(List<string> tickerListTws, int timeout);
        Task<IEnumerable<string>> GetFundamentalData(List<string> tickerList, string reportType, int timeout);
    }
}
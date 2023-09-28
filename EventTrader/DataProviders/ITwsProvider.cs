using IBApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eomn.DataProviders
{
    public interface ITwsProvider
    {
        Task<List<ContractDetails>> GetContractDetails(List<string> tickerListTws, int timeout);
        List<string> ExtractIdsFromContractDetailsList(List<ContractDetails> contractDetailsList);
        Task<IEnumerable<string>> GetFundamentalData(List<string> tickerList, string reportType, int timeout);
    }
}
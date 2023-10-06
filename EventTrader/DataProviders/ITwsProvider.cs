using IBApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eomn.DataProviders
{
    public interface ITwsProvider
    {
        public event Action<string> Status;
        Task<List<ContractDetails>> GetContractDetails(List<string> tickerListTws, int timeout);
        List<string> ExtractIdsFromContractDetailsList(List<ContractDetails> contractDetailsList);
        Task<List<string>> GetFundamentalData(List<string> tickerList, string reportType, int timeout);
        List<string> ExtractRoeFromFundamentalDataList(List<string> fundamentalDataList);
        List<string> ExtractSummaryFromFundamentalDataList(List<string> fundamentalDataList);
    }
}
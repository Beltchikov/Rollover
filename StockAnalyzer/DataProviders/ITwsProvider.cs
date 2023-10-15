using IBApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public interface ITwsProvider
    {
        public event Action<string> Status;
        Task<List<ContractDetails>> GetContractDetails(List<string> contractStringsTws, int timeout);
        List<string> ExtractIdsFromContractDetailsList(List<ContractDetails> contractDetailsList);
        Task<List<string>> GetFundamentalData(List<string> contractStringsTws, string reportType, int timeout);
        List<string> ExtractRoeFromFundamentalDataList(List<string> fundamentalDataList);
        List<string> ExtractSummaryFromFundamentalDataList(List<string> fundamentalDataList);
    }
}
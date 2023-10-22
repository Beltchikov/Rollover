using IBApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        List<string> ExtractPayoutRationYFromFundamentalDataList(List<string> fundamentalDataList);
        List<string> ExtractQuarterlyDataFromFundamentalDataList(List<string> fundamentalDataListPayoutRatio, Action<List<string>, string, XElement?> twiceAYearCalculation, Action<List<string>, string, XElement?> quarterlyCalculations);
        List<string> ExtractNpvYFromFundamentalDataList(List<string> fundamentalDataListPayoutRatio, double riskFreeRate);
        List<string> ExtractSharesOutYFromFundamentalDataList(List<string> fundamentalDataListFinStatements);
        List<string> ExtractSharesOutQFromFundamentalDataList(List<string> fundamentalDataListFinStatements);
        void PayoutRationQuarterlyCalculations(List<string> resultQuarterly, string ticker, XElement? interimStatement);
        void PayoutRatioTwiceAYearCalculations(List<string> resultTwiceAYear, string ticker, XElement? interimStatement);
    }
}
using IBApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockAnalyzer.DataProviders
{
    public interface ITwsProvider
    {
        public event Action<string> Status;
        Task<List<ContractDetails>> GetContractDetails(List<string> contractStringsTws, int timeout);
        List<string> ConIdsFromContractDetailsList(List<ContractDetails> contractDetailsList);
        Task<List<string>> FundamentalDataFromContractStrings(List<string> contractStringsTws, string reportType, int timeout);
        List<string> RoeFromFundamentalDataList(List<string> fundamentalDataList);
        List<string> DesriptionOfCompanyFromFundamentalDataList(List<string> fundamentalDataList);
        List<string> PayoutRatioYFromFundamentalDataList(List<string> fundamentalDataList);
        List<string> QuarterlyDataFromFundamentalDataList(List<string> fundamentalDataListPayoutRatio, Action<List<string>, string, XElement?> twiceAYearCalculation, Action<List<string>, string, XElement?> quarterlyCalculations);
        List<string> SharesOutYFromFundamentalDataList(List<string> fundamentalDataListFinStatements);
        List<string> SharesOutQFromFundamentalDataList(List<string> fundamentalDataListFinStatements);
        void PayoutRatioTwiceAYearCalculations(List<string> resultTwiceAYear, string ticker, XElement? interimStatement);
        void PayoutRatioQuarterlyCalculations(List<string> resultQuarterly, string ticker, XElement? interimStatement);
        void SharesOutTwiceAYearCalculations(List<string> resultTwiceAYear, string ticker, XElement? interimStatement);
        void SharesOutQuarterlyCalculations(List<string> resultQuarterly, string ticker, XElement? interimStatement);
    }
}
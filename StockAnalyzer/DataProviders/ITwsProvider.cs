using IBApi;
using StockAnalyzer.DataProviders.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockAnalyzer.DataProviders
{
    public interface ITwsProvider
    {
        public event Action<string> Status;
        Task<List<ContractDetails>> ContractDetailsListFromContractStringsList(List<string> contractStringsTws, int timeout);
        List<string> ConIdsFromContractDetailsList(List<ContractDetails> contractDetailsList);
        Task<List<DataStringWithTicker>> FundamentalDataFromContractStrings(List<string> contractStringsTws, string reportType, int timeout);
        Task<List<DataStringWithTicker>> MarginFromContractStrings(List<string> contractStringsListTws, int timeout);
        List<string> RoeFromFundamentalDataList(List<DataStringWithTicker> fundamentalDataList);
        List<string> DesriptionOfCompanyFromFundamentalDataList(List<DataStringWithTicker> fundamentalDataList);
        List<string> PayoutRatioYFromFundamentalDataList(List<DataStringWithTicker> fundamentalDataList);
        List<string> QuarterlyDataFromFundamentalDataList(
            List<DataStringWithTicker> fundamentalDataListPayoutRatio,
            Action<List<string>, string, string, XElement?> twiceAYearCalculation,
            Action<List<string>, string, string, XElement?> quarterlyCalculations,
            string statusMessage);
        List<string> SharesOutYFromFundamentalDataList(List<DataStringWithTicker> fundamentalDataListFinStatements);
        void PayoutRatioTwiceAYearCalculations(List<string> resultTwiceAYear, string ticker, string currency, XElement? interimStatement);
        void PayoutRatioQuarterlyCalculations(List<string> resultQuarterly, string ticker, string currency, XElement? interimStatement);
        void SharesOutTwiceAYearCalculations(List<string> resultTwiceAYear, string ticker, string currency, XElement? interimStatement);
        void SharesOutQuarterlyCalculations(List<string> resultQuarterly, string ticker, string currency, XElement? interimStatement);
        Task<List<string>> CurrentPriceFromContractStrings(List<string> contractStringsListTws, int timeout);
    }
}
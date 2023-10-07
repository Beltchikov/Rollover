using Eomn.Ib;
using IBApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Eomn.DataProviders
{
    public class TwsProvider : ProviderBase, ITwsProvider
    {
        private IIbHost _ibHost;

        public TwsProvider(IIbHost ibHost, IIbClientQueue queue)
        {
            _ibHost = ibHost;
        }

        public async Task<List<ContractDetails>> GetContractDetails(List<string> tickerList, int timeout)
        {
            var result = new List<ContractDetails>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                var tickerTrimmed = ticker.Trim();
                TriggerStatus($"Retrieving contract details for {tickerTrimmed} {cnt++}/{tickerList.Count}");
                var contractDetails = await _ibHost.RequestContractDetailsAsync(tickerTrimmed, timeout);
                if (contractDetails != null)
                {
                    result.Add(contractDetails);
                }
            }

            return result;
        }

        public List<string> ExtractIdsFromContractDetailsList(List<ContractDetails> contractDetailsList)
        {
            TriggerStatus($"Extracting contract ids from the contract details");
            var result = new List<string>();

            foreach (ContractDetails contractDetails in contractDetailsList)
            {
                result.Add($"{contractDetails?.Contract.LocalSymbol} {contractDetails?.Contract.ConId}");
            }

            return result;
        }

        public async Task<List<string>> GetFundamentalData(List<string> tickerList, string reportType, int timeout)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                var tickerTrimmed = ticker.Trim();
                TriggerStatus($"Retrieving fundamental data for {tickerTrimmed}, report type: {reportType} {cnt++}/{tickerList.Count}");
                var fundamentalDataString = await _ibHost.RequestFundamentalDataAsync(tickerTrimmed, reportType, timeout);
                result.Add(fundamentalDataString);
            }

            return result;
        }

        public List<string> ExtractRoeFromFundamentalDataList(List<string> fundamentalDataList)
        {
            TriggerStatus($"Extracting ROE from the fundamental data list");
            var result = new List<string>();

            foreach (string fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = ParseXDocumentWithChecks(fundamentalData, result);
                if(xDocument == null) // some error string has been added
                {
                    continue;
                }

                var ratiosElement = xDocument.Descendants("Ratios");
                var groupElements = ratiosElement.Descendants("Group");
                var groupElementOtherRatios = groupElements.FirstOrDefault(e => e.Attribute("ID")?.Value == "Other Ratios");
                var ratioElements = groupElementOtherRatios?.Descendants("Ratio");
                var ratioElementRoe = ratioElements?.FirstOrDefault(e => e.Attribute("FieldName")?.Value == "TTMROEPCT");

                double roeAsDouble = 0;
                if (ratioElementRoe != null)
                {
                    double.TryParse(ratioElementRoe.Value, NumberStyles.Number, new CultureInfo("EN-US"), out roeAsDouble);
                }
                if (roeAsDouble < -99999)
                {
                    roeAsDouble = 0;
                }

                var roeAsString = roeAsDouble == 0 ? null : roeAsDouble.ToString("0.0") + "%";

                var issuesElement = xDocument.Descendants("Issues").FirstOrDefault();
                var issueElements = issuesElement?.Descendants("Issue");
                var issueElementId1 = issueElements?.FirstOrDefault(e => e.Attribute("ID")?.Value == "1");
                var issueIdElements = issueElementId1?.Descendants("IssueID");
                var issueIdElementTicker = issueIdElements?.FirstOrDefault(e => e.Attribute("Type")?.Value == "Ticker");

                result.Add($"{issueIdElementTicker?.Value}\t{roeAsString}");
            }

            return result;
        }

        public List<string> ExtractSummaryFromFundamentalDataList(List<string> fundamentalDataList)
        {
            TriggerStatus($"Extracting business summary from the fundamental data list");
            var result = new List<string>();

            foreach (string fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = ParseXDocumentWithChecks(fundamentalData, result);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }

                var textInfoElement = xDocument.Descendants("TextInfo");
                var textElements = textInfoElement?.Descendants("Text");
                var textElementBusinessSummary = textElements?.FirstOrDefault(e => e.Attribute("Type")?.Value == "Business Summary");
                var textElementFinancialSummary = textElements?.FirstOrDefault(e => e.Attribute("Type")?.Value == "Financial Summary");

                result.Add($"{textElementBusinessSummary?.Value}{Environment.NewLine}" +
                    $"{textElementFinancialSummary?.Value}{Environment.NewLine}{string.Concat(Enumerable.Repeat("-", 20))}");
            }

            return result;
        }

        private XDocument? ParseXDocumentWithChecks(string stringToParse, List<string> result)
        {
            if (stringToParse == null)
            {
                result.Add($"UNKNOWN\t");
                return null;
            }

            XDocument xDocument;
            try
            {
                xDocument = XDocument.Parse(stringToParse);
            }
            catch (XmlException)
            {
                var errorArray = stringToParse.Split(" ");
                result.Add($"{errorArray[0]}\t{errorArray[1]}");
                return null;
            }

            return xDocument;
        }
    }
}

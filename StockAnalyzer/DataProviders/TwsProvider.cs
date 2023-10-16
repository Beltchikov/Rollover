using IBApi;
using IbClient.IbHost;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace StockAnalyzer.DataProviders
{
    public class TwsProvider : ProviderBase, ITwsProvider
    {
        private IIbHost _ibHost;

        public TwsProvider(IIbHost ibHost, IIbHostQueue queue)
        {
            _ibHost = ibHost;
        }

        public async Task<List<ContractDetails>> GetContractDetails(List<string> contractStringsTws, int timeout)
        {
            var result = new List<ContractDetails>();

            int cnt = 1;
            foreach (string contractString in contractStringsTws)
            {
                var contractStringTrimmed = contractString.Trim();
                TriggerStatus($"Retrieving contract details for {contractStringTrimmed} {cnt++}/{contractStringsTws.Count}");
                ContractDetails contractDetails = await ContractDetailsFromContractString(timeout, contractString);
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

        public async Task<List<string>> GetFundamentalData(List<string> contractStringsTws, string reportType, int timeout)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string contractString in contractStringsTws)
            {
                var contractStringTrimmed = contractString.Trim();
                TriggerStatus($"Retrieving fundamental data for {contractStringTrimmed}, report type: {reportType} {cnt++}/{contractStringsTws.Count}");
                string fundamentalDataString = await FundamentalDataFromContractString(timeout, reportType, contractStringTrimmed);
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
                if (xDocument == null) // some error string has been added
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
                string ticker = TickerFromXDocument(xDocument);
                result.Add($"{ticker}\t{roeAsString}");
            }

            return result;
        }

        private static string TickerFromXDocument(XDocument? xDocument)
        {
            string ticker;
            var issuesElement = xDocument?.Descendants("Issues").FirstOrDefault();
            var issueElements = issuesElement?.Descendants("Issue");
            var issueElementId1 = issueElements?.FirstOrDefault(e => e.Attribute("ID")?.Value == "1");
            var issueIdElements = issueElementId1?.Descendants("IssueID");
            var issueIdElementTicker = issueIdElements?.FirstOrDefault(e => e.Attribute("Type")?.Value == "Ticker");
            ticker = issueIdElementTicker?.Value ?? "";
            return ticker;
        }

        public List<string> ExtractPayoutRationYFromFundamentalDataList(List<string> fundamentalDataList)
        {
            TriggerStatus($"Extracting Net Income from the fundamental data list");
            var result = new List<string>();
            result.Add($"Ticker\tNet Income in M\tDiv. Paid\tPayback Ratio");

            foreach (string fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = ParseXDocumentWithChecks(fundamentalData, result);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }
                IEnumerable<XElement>? statementSection = ExtractStatementSection(xDocument);
                
                double netIncome = ExtractNetIncome(statementSection);
                double divPaid = ExtractDividendsPaid(statementSection);
                double paybackRatio = CalculatePaybackRatio(netIncome, divPaid);

                string ticker = TickerFromXDocument(xDocument);
                result.Add($"{ticker}\t{netIncome}\t{divPaid}\t{paybackRatio}%");
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

        private async Task<ContractDetails> ContractDetailsFromContractString(int timeout, string contractString)
        {
            ContractDetails contractDetails = null!;
            var contractArray = contractString.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (contractArray.Length == 1)
            {
                contractDetails = await _ibHost.RequestContractDetailsAsync(contractArray[0]?.Trim(), timeout);
            }
            else if (contractArray.Length == 2)
            {
                contractDetails = await _ibHost.RequestContractDetailsAsync(contractArray[0]?.Trim(), timeout, contractArray[1]?.Trim());
            }
            else if (contractArray.Length == 3)
            {
                contractDetails = await _ibHost.RequestContractDetailsAsync(contractArray[0]?.Trim(), timeout, contractArray[1]?.Trim(), contractArray[2]?.Trim());
            }
            else if (contractArray.Length == 4)
            {
                contractDetails = await _ibHost.RequestContractDetailsAsync(
                    contractArray[0]?.Trim(),
                    timeout,
                    contractArray[1]?.Trim(),
                    contractArray[2]?.Trim(),
                    contractArray[3]?.Trim());
            }
            else
            {
                throw new ApplicationException("Wrong number of elements in contract's string representation.");
            }

            return contractDetails;
        }

        private async Task<string> FundamentalDataFromContractString(int timeout, string reportType, string contractString)
        {
            string fundamentalData;
            var contractArray = contractString.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (contractArray.Length == 1)
            {
                fundamentalData = await _ibHost.RequestFundamentalDataAsync(contractArray[0]?.Trim(), reportType, timeout);
            }
            else if (contractArray.Length == 2)
            {
                fundamentalData = await _ibHost.RequestFundamentalDataAsync(
                    contractArray[0]?.Trim(),
                    reportType,
                    timeout,
                    contractArray[1]?.Trim());
            }
            else if (contractArray.Length == 3)
            {
                fundamentalData = await _ibHost.RequestFundamentalDataAsync(contractArray[0]?.Trim(), reportType, timeout, contractArray[1]?.Trim(), contractArray[2]?.Trim());
            }
            else if (contractArray.Length == 4)
            {
                fundamentalData = await _ibHost.RequestFundamentalDataAsync(
                    contractArray[0]?.Trim(),
                    reportType,
                    timeout,
                    contractArray[1]?.Trim(),
                    contractArray[2]?.Trim(),
                    contractArray[3]?.Trim());
            }
            else
            {
                throw new ApplicationException("Wrong number of elements in contract's string representation.");
            }
            return fundamentalData;
        }

        private static IEnumerable<XElement>? ExtractStatementSection(XDocument? xDocument)
        {
            IEnumerable<XElement>? statementElements;
            var annualPeriodsElement = xDocument?.Descendants("AnnualPeriods");
            var fiscalPeriodElements = annualPeriodsElement?.Descendants("FiscalPeriod");
            var lastFiscalPeriodElement = fiscalPeriodElements?.MaxBy(e => Convert.ToInt32(e.Attribute("FiscalYear")?.Value));
            statementElements = lastFiscalPeriodElement?.Descendants("Statement");
            return statementElements;
        }

        private static double CalculatePaybackRatio(double netIncome, double divPaid)
        {
            double paybackRatio;
            netIncome = netIncome == 0 ? 1 : netIncome;
            paybackRatio = (divPaid / netIncome) * 100;
            paybackRatio = Math.Round(paybackRatio, 1);
            return paybackRatio;
        }

        private static double ExtractDividendsPaid(IEnumerable<XElement>? statementSection)
        {
            double divPaid;
            var casStatementElement = statementSection?.Where(e => e?.Attribute("Type")?.Value == "CAS");
            var lineItemElementsCas = casStatementElement?.Descendants("lineItem");
            var fcdpLineItemElement = lineItemElementsCas?.Where(e => e?.Attribute("coaCode")?.Value == "FCDP").FirstOrDefault();
            var divPaidNegative = fcdpLineItemElement?.Value;
            divPaid = Convert.ToDouble(divPaidNegative, CultureInfo.InvariantCulture) * -1;
            return divPaid;
        }

        private static double ExtractNetIncome(IEnumerable<XElement>? statementElements)
        {
            double netIncome;
            var incStatementElement = statementElements?.Where(e => e?.Attribute("Type")?.Value == "INC");
            var lineItemElementsInc = incStatementElement?.Descendants("lineItem");
            var nincLineItemElement = lineItemElementsInc?.Where(e => e?.Attribute("coaCode")?.Value == "NINC").FirstOrDefault();
            var netIncomeAsString = nincLineItemElement?.Value;
            netIncome = Convert.ToDouble(netIncomeAsString, CultureInfo.InvariantCulture);
            return netIncome;
        }

    }
}

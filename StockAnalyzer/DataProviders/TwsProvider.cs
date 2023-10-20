using IBApi;
using IbClient.IbHost;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

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
            TriggerStatus($"Extracting Payout Ratio (Y) from the fundamental data list");
            var result = new List<string>();
            result.Add($"Ticker\tNet Income (Y) in M\tDiv. Paid\tPayback Ratio");

            foreach (string fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = ParseXDocumentWithChecks(fundamentalData, result);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }
                IEnumerable<XElement>? statementSection = ExtractStatementSection(xDocument, "AnnualPeriods");

                double netIncome = ExtractNetIncome(statementSection);
                double divPaid = ExtractDividendsPaid(statementSection);
                double paybackRatio = CalculatePaybackRatio(netIncome, divPaid);

                string ticker = TickerFromXDocument(xDocument);
                result.Add($"{ticker}\t{netIncome}\t{divPaid}\t{paybackRatio}%");
            }

            return result;
        }


        public List<string> ExtractPayoutRatioQFromFundamentalDataList(List<string> fundamentalDataList)
        {
            TriggerStatus($"Extracting Payout Ratio (Q) from the fundamental data list");
            var result = new List<string>();
            
            foreach (string fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = ParseXDocumentWithChecks(fundamentalData, result);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }
                string ticker = TickerFromXDocument(xDocument);

                IEnumerable<XElement>? interimStatements = ExtractAllStatements(xDocument, "InterimPeriods");
                var interimStatement = interimStatements?.FirstOrDefault();
                bool twiceAYear = ReportingFrequencyIsTwiceAYear(interimStatement);  // otherwise quarterly

                //double netIncomeQ4, divPaidQ4, paybackRatioQ4;
                //double netIncomeQ3, divPaidQ3, paybackRatioQ3;
                //double netIncomeQ2, divPaidQ2, paybackRatioQ2;
                //double netIncomeQ1, divPaidQ1, paybackRatioQ1;
                double netIncomeH2, divPaidH2, paybackRatioH2;
                double netIncomeH1, divPaidH1, paybackRatioH1;
                double netIncomeTtm, divPaidTtm, paybackRatioTtm;
                if (twiceAYear)
                {
                    netIncomeH1 = ExtractNetIncome(interimStatement, 0);
                    netIncomeH2 = ExtractNetIncome(interimStatement, 1);
                    netIncomeTtm = netIncomeH1 + netIncomeH2;
                    divPaidH1 = ExtractDividendsPaid(interimStatement, 0);
                    divPaidH2 = ExtractDividendsPaid(interimStatement, 1);
                    divPaidTtm = divPaidH1 + divPaidH2; 
                    paybackRatioH1 = CalculatePaybackRatio(netIncomeH1, divPaidH1);
                    paybackRatioH2 = CalculatePaybackRatio(netIncomeH2, divPaidH2);
                    paybackRatioTtm = CalculatePaybackRatio(netIncomeTtm, divPaidTtm);

                    if (!result.Any()) result.Add($"Ticker\tH2 Net Inc in M\tH2 Div\tH2 Ratio\tH1 Net Inc\tH1 Div\tH1 Ratio" +
                        $"\tTTM Net Inc\tTTM Div\tTTM Ratio");
                    result.Add($"{ticker}\t{netIncomeH2}\t{divPaidH2}\t{paybackRatioH2}%\t{netIncomeH1}\t{divPaidH1}\t{paybackRatioH1}%" +
                        $"\t{netIncomeTtm}\t{divPaidTtm}\t{paybackRatioTtm}%");
                }
                else
                {

                }

                //double netIncome = ExtractNetIncome(statementSection);
                //double divPaid = ExtractDividendsPaid(statementSection);
                //double paybackRatio = CalculatePaybackRatio(netIncome, divPaid);


                //result.Add($"{ticker}\t{netIncome}\t{divPaid}\t{paybackRatio}%");
            }

            return result;
        }

        public IEnumerable<string> ExtractNpvYFromFundamentalDataList(List<string> fundamentalDataList, double riskFreeRate)
        {
            TriggerStatus($"Extracting NPV from the fundamental data list");
            var result = new List<string>();
            result.Add($"Ticker\tDiv. Paid\tCommon\tPreferred\tTotal Shares\tDPS\tNPV\tCurrency");

            foreach (string fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = ParseXDocumentWithChecks(fundamentalData, result);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }
                IEnumerable<XElement>? statementSection = ExtractStatementSection(xDocument, "AnnualPeriods");

                double divPaid = ExtractDividendsPaid(statementSection);

                double commonStocks = ExtractTotalSharesOutstanding(statementSection, "QTCO");
                double preferredStocks = ExtractTotalSharesOutstanding(statementSection, "QTPO");
                double totalShares = commonStocks + preferredStocks;

                double dps = divPaid / commonStocks;
                double dpsRounded = Math.Round(dps, 5);
                double npv = dps / (riskFreeRate / 100);
                npv = Math.Round(npv, 2);

                string? currency = ExtractCurrency(xDocument);

                string ticker = TickerFromXDocument(xDocument);
                result.Add($"{ticker}\t{divPaid}\t{commonStocks}\t{preferredStocks}\t{totalShares}\t{dpsRounded}\t{npv}\t{currency}");
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

        private static IEnumerable<XElement>? ExtractStatementSection(XDocument? xDocument, string periods)
        {
            IEnumerable<XElement>? statementElements;
            var annualPeriodsElement = xDocument?.Descendants(periods);
            var fiscalPeriodElements = annualPeriodsElement?.Descendants("FiscalPeriod");
            var lastFiscalPeriodElement = fiscalPeriodElements?.MaxBy(e => Convert.ToInt32(e.Attribute("FiscalYear")?.Value));
            statementElements = lastFiscalPeriodElement?.Descendants("Statement");
            return statementElements;
        }

        private IEnumerable<XElement>? ExtractAllStatements(XDocument xDocument, string periods)
        {
            return xDocument?.Descendants(periods);
        }

        private bool ReportingFrequencyIsTwiceAYear(XElement? interimStatement)
        {
            DateTime endDate0 = EndDateOfFiscalPeriod(interimStatement, 0);
            DateTime endDate1 = EndDateOfFiscalPeriod(interimStatement, 1);

            if (ConditionTwiceAYear(endDate0, endDate1)) // twice a year
            {
                // check
                DateTime endDate2 = EndDateOfFiscalPeriod(interimStatement, 2);
                if (!ConditionTwiceAYear(endDate1, endDate2))
                {
                    throw new ApplicationException($"Reporting frequency is twice a year, but the third statement does not fit.");
                }
                return true;
            }
            else
            {
                DateTime endDate2 = EndDateOfFiscalPeriod(interimStatement, 2);
                if (ConditionTwiceAYear(endDate1, endDate2))
                {
                    throw new ApplicationException($"Reporting frequency is quarterly, but the third statement does not fit.");
                }
                DateTime endDate3 = EndDateOfFiscalPeriod(interimStatement, 3);
                if (ConditionTwiceAYear(endDate2, endDate3))
                {
                    throw new ApplicationException($"Reporting frequency is quarterly, but the forth statement does not fit.");
                }
                return false;
            }
        }

        private static bool ConditionTwiceAYear(DateTime endDate0, DateTime endDate1)
        {
            return 170 < (endDate0 - endDate1).TotalDays && (endDate0 - endDate1).TotalDays < 200;
        }

        private static DateTime EndDateOfFiscalPeriod(XElement? interimStatement, int periodsAgo)
        {
            DateTime endDate;
            var endDateString = interimStatement?.Descendants("FiscalPeriod").Skip(periodsAgo).FirstOrDefault()?.Attribute("EndDate")?.Value;
            if (!DateTime.TryParse(endDateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                throw new ApplicationException($"Can not parce the DateTime from {endDateString}");
            }

            return endDate;
        }

        private string? ExtractCurrency(XDocument xDocument)
        {
            var reportingCurrencyElement = xDocument?.Descendants("ReportingCurrency").FirstOrDefault();
            return reportingCurrencyElement?.Attribute("Code")?.Value;
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

        private double ExtractDividendsPaid(XElement? periodsElement, int periodsAgo)
        {
            double divPaid;

            var fiscalPeriodElements = periodsElement?.Descendants("FiscalPeriod");
            var fiscalPeriodElement = fiscalPeriodElements?.Skip(periodsAgo).FirstOrDefault();
            var statementElements = fiscalPeriodElement?.Descendants("Statement");
            var casStatementElement = statementElements?.Where(e => e?.Attribute("Type")?.Value == "CAS");
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

        private static double ExtractNetIncome(XElement? periodsElement, int periodsAgo)
        {
            double netIncome;

            var fiscalPeriodElements = periodsElement?.Descendants("FiscalPeriod");
            var fiscalPeriodElement = fiscalPeriodElements?.Skip(periodsAgo).FirstOrDefault();
            var statementElements = fiscalPeriodElement?.Descendants("Statement");
            var incStatementElement = statementElements?.Where(e => e?.Attribute("Type")?.Value == "INC");
            var lineItemElementsInc = incStatementElement?.Descendants("lineItem");
            var nincLineItemElement = lineItemElementsInc?.Where(e => e?.Attribute("coaCode")?.Value == "NINC").FirstOrDefault();
            var netIncomeAsString = nincLineItemElement?.Value;
            netIncome = Convert.ToDouble(netIncomeAsString, CultureInfo.InvariantCulture);
            return netIncome;
        }

        /// <summary>
        /// coaCode: QTCO for common shares; QTPO for preferred shares
        /// </summary>
        /// <param name="statementSection"></param>
        /// <param name="coaCode"></param>
        /// <returns></returns>
        private static double ExtractTotalSharesOutstanding(IEnumerable<XElement>? statementSection, string coaCode)
        {
            double shares = 0;
            var balStatementElement = statementSection?.Where(e => e?.Attribute("Type")?.Value == "BAL");
            var lineItemElementsInc = balStatementElement?.Descendants("lineItem");
            var qtcoLineItemElement = lineItemElementsInc?.Where(e => e?.Attribute("coaCode")?.Value == coaCode).FirstOrDefault();
            var sharesAsString = qtcoLineItemElement?.Value;
            shares = Convert.ToDouble(sharesAsString, CultureInfo.InvariantCulture);
            return shares;
        }
    }
}

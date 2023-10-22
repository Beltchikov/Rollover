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

        public List<string> ConIdsFromContractDetailsList(List<ContractDetails> contractDetailsList)
        {
            TriggerStatus($"Extracting contract ids from the contract details");
            var result = new List<string>();

            foreach (ContractDetails contractDetails in contractDetailsList)
            {
                result.Add($"{contractDetails?.Contract.LocalSymbol} {contractDetails?.Contract.ConId}");
            }

            return result;
        }

        public async Task<List<string>> FundamentalDataFromContractStrings(List<string> contractStringsTws, string reportType, int timeout)
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

        public List<string> RoeFromFundamentalDataList(List<string> fundamentalDataList)
        {
            TriggerStatus($"Extracting ROE from the fundamental data list");
            var result = new List<string>();

            foreach (string fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = XDocumentFromStringWithChecks(fundamentalData, result);
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

        public List<string> PayoutRatioYFromFundamentalDataList(List<string> fundamentalDataList)
        {
            TriggerStatus($"Extracting Payout Ratio (Y) from the fundamental data list");
            var result = new List<string>();
            result.Add($"Ticker\tNet Income (Y) in M\tDiv. Paid\tPayback Ratio");

            foreach (string fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = XDocumentFromStringWithChecks(fundamentalData, result);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }
                IEnumerable<XElement>? statementSection = StatementElementsFromXDocument(xDocument, "AnnualPeriods");

                double netIncome = NetIncomeFromFiscalPeriodElement(statementSection);
                double divPaid = DividendsPaidFromFiscalPeriodElement(statementSection);
                double paybackRatio = PaybackRatioFromNetIncomeAndDividends(netIncome, divPaid);

                string ticker = TickerFromXDocument(xDocument);
                result.Add($"{ticker}\t{netIncome}\t{divPaid}\t{paybackRatio}%");
            }

            return result;
        }


        public List<string> QuarterlyDataFromFundamentalDataList(
            List<string> fundamentalDataList,
            Action<List<string>, string, XElement?> twiceAYearCalculations,
            Action<List<string>, string, XElement?> quarterlyCalculations, 
            string statusMessage)
        {
            TriggerStatus(statusMessage);
            List<string> resultQuarterly = new();
            List<string> resultTwiceAYear = new();

            foreach (string fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = XDocumentFromStringWithChecks(fundamentalData, resultQuarterly);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }
                string ticker = TickerFromXDocument(xDocument);

                IEnumerable<XElement>? interimStatements = PeriodsElementFromXDocument(xDocument, "InterimPeriods");
                var interimStatement = interimStatements?.FirstOrDefault();

                bool twiceAYear = ReportingFrequencyIsTwiceAYear(interimStatement);  // otherwise quarterly
                if (twiceAYear)
                {
                    twiceAYearCalculations(resultTwiceAYear, ticker, interimStatement);
                }
                else
                {
                    quarterlyCalculations(resultQuarterly, ticker, interimStatement);
                }
            }
            List<string> result = ResultListFromTwoDifferentlyStructuredLists(resultQuarterly, resultTwiceAYear);
            return result;
        }

        public List<string> SharesOutYFromFundamentalDataList(List<string> fundamentalDataList)
        {
            TriggerStatus($"Extracting Total Shares Outstanding (Y) from the fundamental data list");
            var result = new List<string>();
            result.Add($"Ticker\tCommon (Y) in M\tPreferred\tTotal");

            foreach (string fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = XDocumentFromStringWithChecks(fundamentalData, result);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }
                string ticker = TickerFromXDocument(xDocument);

                IEnumerable<XElement>? statementSection = StatementElementsFromXDocument(xDocument, "AnnualPeriods");
                double commonSharesOut = SharesOutstandingFromFiscalPeriodElement(statementSection, "QTCO");
                double preferredSharesOut = SharesOutstandingFromFiscalPeriodElement(statementSection, "QTPO");
                double totalSharesOut = commonSharesOut + preferredSharesOut;

                result.Add($"{ticker}\t{commonSharesOut}\t{preferredSharesOut}\t{totalSharesOut}");
            }

            return result;
        }

        public List<string> SharesOutQFromFundamentalDataList(List<string> fundamentalDataList)
        {
            TriggerStatus($"Extracting Total Shares Outstanding (Q) from the fundamental data list");
            var result = new List<string>();
            //result.Add($"Ticker\tNet Income (Y) in M\tDiv. Paid\tPayback Ratio");

            MessageBox.Show("ExtractSharesOutQFromFundamentalDataList");

            foreach (string fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = XDocumentFromStringWithChecks(fundamentalData, result);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }
            }

            return result;
        }

        public List<string> DesriptionOfCompanyFromFundamentalDataList(List<string> fundamentalDataList)
        {
            TriggerStatus($"Extracting business summary from the fundamental data list");
            var result = new List<string>();

            foreach (string fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = XDocumentFromStringWithChecks(fundamentalData, result);
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

        public void PayoutRatioTwiceAYearCalculations(List<string> resultTwiceAYear, string ticker, XElement? interimStatement)
        {
            double netIncomeH1 = NetIncomeFromPeriodsElement(interimStatement, 0);
            double netIncomeH2 = NetIncomeFromPeriodsElement(interimStatement, 1);
            double netIncomeTtm = netIncomeH1 + netIncomeH2;
            double divPaidH1 = DividendsPaidFromPeriodsElement(interimStatement, 0);
            double divPaidH2 = DividendsPaidFromPeriodsElement(interimStatement, 1);
            double divPaidTtm = divPaidH1 + divPaidH2;
            double paybackRatioH1 = PaybackRatioFromNetIncomeAndDividends(netIncomeH1, divPaidH1);
            double paybackRatioH2 = PaybackRatioFromNetIncomeAndDividends(netIncomeH2, divPaidH2);
            double paybackRatioTtm = PaybackRatioFromNetIncomeAndDividends(netIncomeTtm, divPaidTtm);

            if (!resultTwiceAYear.Any()) resultTwiceAYear.Add($"Ticker\tH2 Net Inc in M\tH2 Div\tH2 Ratio\tH1 Net Inc\tH1 Div\tH1 Ratio" +
                $"\tTTM Net Inc\tTTM Div\tTTM Ratio");
            resultTwiceAYear.Add($"{ticker}\t{netIncomeH2}\t{divPaidH2}\t{paybackRatioH2}%\t{netIncomeH1}\t{divPaidH1}\t{paybackRatioH1}%" +
                $"\t{netIncomeTtm}\t{divPaidTtm}\t{paybackRatioTtm}%");
        }

        public void PayoutRatioQuarterlyCalculations(List<string> resultQuarterly, string ticker, XElement? interimStatement)
        {
            double netIncomeQ1 = NetIncomeFromPeriodsElement(interimStatement, 0);
            double netIncomeQ2 = NetIncomeFromPeriodsElement(interimStatement, 1);
            double netIncomeQ3 = NetIncomeFromPeriodsElement(interimStatement, 2);
            double netIncomeQ4 = NetIncomeFromPeriodsElement(interimStatement, 3);
            double netIncomeTtm = netIncomeQ1 + netIncomeQ2 + netIncomeQ3 + netIncomeQ4;
            double divPaidQ1 = DividendsPaidFromPeriodsElement(interimStatement, 0);
            double divPaidQ2 = DividendsPaidFromPeriodsElement(interimStatement, 1);
            double divPaidQ3 = DividendsPaidFromPeriodsElement(interimStatement, 2);
            double divPaidQ4 = DividendsPaidFromPeriodsElement(interimStatement, 3);
            double divPaidTtm = divPaidQ1 + divPaidQ2 + divPaidQ3 + divPaidQ4;
            double paybackRatioQ1 = PaybackRatioFromNetIncomeAndDividends(netIncomeQ1, divPaidQ1);
            double paybackRatioQ2 = PaybackRatioFromNetIncomeAndDividends(netIncomeQ2, divPaidQ2);
            double paybackRatioQ3 = PaybackRatioFromNetIncomeAndDividends(netIncomeQ3, divPaidQ3);
            double paybackRatioQ4 = PaybackRatioFromNetIncomeAndDividends(netIncomeQ4, divPaidQ4);
            double paybackRatioTtm = PaybackRatioFromNetIncomeAndDividends(netIncomeTtm, divPaidTtm);

            if (!resultQuarterly.Any()) resultQuarterly.Add($"Ticker\tQ4 Net Inc in M\tQ4 Div\tQ4 Ratio\tQ3 Net Inc\tQ3 Div\tQ3 Ratio" +
                $"\tQ2 Net Inc\tQ2 Div\tQ2 Ratio\tQ1 Net Inc\tQ1 Div\tQ1 Ratio" +
                $"\tTTM Net Inc\tTTM Div\tTTM Ratio");
            resultQuarterly.Add($"{ticker}\t{netIncomeQ4}\t{divPaidQ4}\t{paybackRatioQ4}%\t{netIncomeQ3}\t{divPaidQ3}\t{paybackRatioQ3}%" +
                $"\t{netIncomeQ2}\t{divPaidQ2}\t{paybackRatioQ2}%\t{netIncomeQ1}\t{divPaidQ1}\t{paybackRatioQ1}%" +
                $"\t{netIncomeTtm}\t{divPaidTtm}\t{paybackRatioTtm}%");
        }

        /// <summary>
        /// SharesOutTwiceAYearCalculations
        /// </summary>
        /// <param name="resultTwiceAYear"></param>
        /// <param name="ticker"></param>
        /// <param name="periodsElement">AnnualPeriods or InterimPeriods</param>
        public void SharesOutTwiceAYearCalculations(List<string> resultTwiceAYear, string ticker, XElement? periodsElement)
        {
            if (!resultTwiceAYear.Any()) resultTwiceAYear.Add($"Ticker\tCommon in M\tPreferred\tTotal");

            double commonSharesOutH1 = SharesOutstandingFromPeriodsElement(periodsElement, "QTCO", 0);
            double preferredSharesOutH1 = SharesOutstandingFromPeriodsElement(periodsElement, "QTPO", 0);
            double totalSharesOutH1 = commonSharesOutH1 + preferredSharesOutH1;
            // No need for TTM value
            resultTwiceAYear.Add($"{ticker}\t{commonSharesOutH1}\t{preferredSharesOutH1}\t{totalSharesOutH1}");
        }

        /// <summary>
        /// SharesOutQuarterlyCalculations
        /// </summary>
        /// <param name="resultQuarterly"></param>
        /// <param name="ticker"></param>
        /// <param name="periodsElement">AnnualPeriods or InterimPeriods</param>
        public void SharesOutQuarterlyCalculations(List<string> resultQuarterly, string ticker, XElement? periodsElement)
        {
            // No need for TTM value
            SharesOutTwiceAYearCalculations(resultQuarterly, ticker, periodsElement);
        }

        private XDocument? XDocumentFromStringWithChecks(string stringToParse, List<string> result)
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

        private static IEnumerable<XElement>? StatementElementsFromXDocument(XDocument? xDocument, string periods)
        {
            IEnumerable<XElement>? statementElements;
            var annualPeriodsElement = xDocument?.Descendants(periods);
            var fiscalPeriodElements = annualPeriodsElement?.Descendants("FiscalPeriod");
            var lastFiscalPeriodElement = fiscalPeriodElements?.MaxBy(e => Convert.ToInt32(e.Attribute("FiscalYear")?.Value));
            statementElements = lastFiscalPeriodElement?.Descendants("Statement");
            return statementElements;
        }

        /// <summary>
        /// </summary>
        /// <param name="xDocument"></param>
        /// <param name="periods">InterimPeriods or AnnualPeriods</param>
        /// <returns></returns>
        private IEnumerable<XElement>? PeriodsElementFromXDocument(XDocument xDocument, string periods)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodsElement"></param>
        /// <param name="periodsAgo">AnnualPeriods or InterimPeriods</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private static DateTime EndDateOfFiscalPeriod(XElement? periodsElement, int periodsAgo)
        {
            var endDateString = periodsElement?.Descendants("FiscalPeriod").Skip(periodsAgo).FirstOrDefault()?.Attribute("EndDate")?.Value;
            if (!DateTime.TryParse(endDateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                throw new ApplicationException($"Can not parce the DateTime from {endDateString}");
            }
            return endDate;
        }

        private string? CurrencyFromXmlDocument(XDocument xDocument)
        {
            var reportingCurrencyElement = xDocument?.Descendants("ReportingCurrency").FirstOrDefault();
            return reportingCurrencyElement?.Attribute("Code")?.Value;
        }

        private static double PaybackRatioFromNetIncomeAndDividends(double netIncome, double divPaid)
        {
            double paybackRatio;
            netIncome = netIncome == 0 ? 1 : netIncome;
            paybackRatio = (divPaid / netIncome) * 100;
            paybackRatio = Math.Round(paybackRatio, 1);
            return paybackRatio;
        }

        private static double DividendsPaidFromFiscalPeriodElement(IEnumerable<XElement>? fiscalPeriodElement)
        {
            var casStatementElement = fiscalPeriodElement?.Where(e => e?.Attribute("Type")?.Value == "CAS");
            var lineItemElementsCas = casStatementElement?.Descendants("lineItem");
            var fcdpLineItemElement = lineItemElementsCas?.Where(e => e?.Attribute("coaCode")?.Value == "FCDP").FirstOrDefault();
            var divPaidNegative = fcdpLineItemElement?.Value;
            double divPaid = Convert.ToDouble(divPaidNegative, CultureInfo.InvariantCulture) * -1;
            return divPaid;
        }

        /// <summary>
        /// ExtractDividendsPaid
        /// </summary>
        /// <param name="periodsElement">AnnualPeriods or InterimPeriods</param>
        /// <param name="periodsAgo"></param>
        /// <returns></returns>
        private double DividendsPaidFromPeriodsElement(XElement? periodsElement, int periodsAgo)
        {
            var fiscalPeriodElements = periodsElement?.Descendants("FiscalPeriod");
            var fiscalPeriodElement = fiscalPeriodElements?.Skip(periodsAgo).FirstOrDefault();
            var statementElements = fiscalPeriodElement?.Descendants("Statement");
            var casStatementElement = statementElements?.Where(e => e?.Attribute("Type")?.Value == "CAS");
            var lineItemElementsCas = casStatementElement?.Descendants("lineItem");
            var fcdpLineItemElement = lineItemElementsCas?.Where(e => e?.Attribute("coaCode")?.Value == "FCDP").FirstOrDefault();
            var divPaidNegative = fcdpLineItemElement?.Value;
            double divPaid = Convert.ToDouble(divPaidNegative, CultureInfo.InvariantCulture) * -1;
            return divPaid;
        }

        private static double NetIncomeFromFiscalPeriodElement(IEnumerable<XElement>? fiscalPeriodElement)
        {
            double netIncome;
            var incStatementElement = fiscalPeriodElement?.Where(e => e?.Attribute("Type")?.Value == "INC");
            var lineItemElementsInc = incStatementElement?.Descendants("lineItem");
            var nincLineItemElement = lineItemElementsInc?.Where(e => e?.Attribute("coaCode")?.Value == "NINC").FirstOrDefault();
            var netIncomeAsString = nincLineItemElement?.Value;
            netIncome = Convert.ToDouble(netIncomeAsString, CultureInfo.InvariantCulture);
            return netIncome;
        }

        /// <summary>
        /// Common shares : coaCode QTCO
        /// Preferred shares : coaCode QTPO
        /// </summary>
        /// <param name="fiscalPeriodElement"></param>
        /// <param name="coaCode"></param>
        /// <returns></returns>
        private static double SharesOutstandingFromFiscalPeriodElement(IEnumerable<XElement>? fiscalPeriodElement, string coaCode)
        {
            var balStatementElement = fiscalPeriodElement?.Where(e => e?.Attribute("Type")?.Value == "BAL");
            var lineItemElements = balStatementElement?.Descendants("lineItem");
            var qtcoLineItemElement = lineItemElements?.Where(e => e?.Attribute("coaCode")?.Value == coaCode).FirstOrDefault();
            var commonSharesOutAsString = qtcoLineItemElement?.Value;
            double commonSharesOut = Convert.ToDouble(commonSharesOutAsString, CultureInfo.InvariantCulture);
            return commonSharesOut;
        }

        /// <summary>
        /// </summary>
        /// <param name="periodsElement">AnnualPeriods or InterimPeriods</param>
        /// <param name="coaCode"></param>
        /// <param name="periodsAgo"></param>
        /// <returns></returns>
        private static double SharesOutstandingFromPeriodsElement(XElement? periodsElement, string coaCode, int periodsAgo)
        {
            var fiscalPeriodElements = periodsElement?.Descendants("FiscalPeriod");
            var fiscalPeriodElement = fiscalPeriodElements?.Skip(periodsAgo).FirstOrDefault();
            var statementElements = fiscalPeriodElement?.Descendants("Statement");
            var balStatementElement = statementElements?.Where(e => e?.Attribute("Type")?.Value == "BAL");
            var lineItemElements = balStatementElement?.Descendants("lineItem");
            var sharesOutLineItemElement = lineItemElements?.Where(e => e?.Attribute("coaCode")?.Value == coaCode).FirstOrDefault();
            var shareOutAsString = sharesOutLineItemElement?.Value;
            var sharesOut = Convert.ToDouble(shareOutAsString, CultureInfo.InvariantCulture);
            return sharesOut;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodsElement">AnnualPeriods or InterimPeriods</param>
        /// <param name="periodsAgo"></param>
        /// <returns></returns>
        private static double NetIncomeFromPeriodsElement(XElement? periodsElement, int periodsAgo)
        {
            var fiscalPeriodElements = periodsElement?.Descendants("FiscalPeriod");
            var fiscalPeriodElement = fiscalPeriodElements?.Skip(periodsAgo).FirstOrDefault();
            var statementElements = fiscalPeriodElement?.Descendants("Statement");
            var incStatementElement = statementElements?.Where(e => e?.Attribute("Type")?.Value == "INC");
            var lineItemElementsInc = incStatementElement?.Descendants("lineItem");
            var nincLineItemElement = lineItemElementsInc?.Where(e => e?.Attribute("coaCode")?.Value == "NINC").FirstOrDefault();
            var netIncomeAsString = nincLineItemElement?.Value;
            double netIncome = Convert.ToDouble(netIncomeAsString, CultureInfo.InvariantCulture);
            return netIncome;
        }

        private static List<string> ResultListFromTwoDifferentlyStructuredLists(List<string> resultQuarterly, List<string> resultTwiceAYear)
        {
            List<string> result;
            if (resultTwiceAYear.Any())
            {
                result = resultTwiceAYear;
                result.Add(Environment.NewLine);
                result.AddRange(resultQuarterly);
            }
            else
            {
                result = resultQuarterly;
                result.Add(Environment.NewLine);
                result.AddRange(resultTwiceAYear);
            }

            return result;
        }
    }
}

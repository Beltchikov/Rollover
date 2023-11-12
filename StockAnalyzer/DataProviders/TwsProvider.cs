using IBApi;
using IbClient.IbHost;
using IbClient.Types;
using StockAnalyzer.DataProviders.FinancialStatements.Tws.Accounts;
using StockAnalyzer.DataProviders.FinancialStatements.Tws.ComputedFinancials;
using StockAnalyzer.DataProviders.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TickType = IbClient.Types.TickType;

namespace StockAnalyzer.DataProviders
{
    public class TwsProvider : ProviderBase, ITwsProvider
    {
        private const string NO_FUNDAMENTAL_DATA = "NO_FUNDAMENTAL_DATA";
        private IIbHost _ibHost;

        public TwsProvider(IIbHost ibHost, IIbHostQueue queue)
        {
            _ibHost = ibHost;
        }

        public async Task<List<ContractDetails>> ContractDetailsListFromContractStringsList(List<string> contractStringsTws, int timeout)
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

        public async Task<List<DataStringWithTicker>> FundamentalDataFromContractStrings(List<string> contractStringsTws, string reportType, int timeout)
        {
            var result = new List<DataStringWithTicker>();

            int cnt = 1;
            foreach (string contractString in contractStringsTws)
            {
                if (string.IsNullOrWhiteSpace(contractString))
                {
                    continue;
                }

                var contractStringTrimmed = contractString.Trim();
                Contract contract = ContractFromString(contractStringTrimmed);
                TriggerStatus($"Retrieving fundamental data for {contractStringTrimmed}, report type: {reportType} {cnt++}/{contractStringsTws.Count}");
                string fundamentalDataString = await FundamentalDataFromContract(timeout, reportType, contract);
                result.Add(new DataStringWithTicker(contract.Symbol, fundamentalDataString));
            }

            return result;
        }

        /// <summary>
        /// See https://interactivebrokers.github.io/tws-api/tick_types.html
        /// </summary>
        /// <param name="contractStringsListTws"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<string>> CurrentPriceFromContractStrings(List<string> contractStringsListTws, int timeout)
        {
            var result = new List<string>();
            result.Add($"Ticker\tPrice\tCurrency\tMarket Data Type\tTick Type\tComment");

            int cnt = 1;
            foreach (string contractString in contractStringsListTws)
            {
                var contractStringTrimmed = contractString.Trim();
                Contract contract = ContractFromString(contractStringTrimmed);
                TriggerStatus($"Retrieving current price for {contractStringTrimmed} {cnt++}/{contractStringsListTws.Count}");


                MarketDataType[] marketDataTypes = new[] { MarketDataType.Live };
                TickType[] tickTypes = new[] { TickType.BidPrice };
                string[] comments = new[] { "LIVE BID" };

                try
                {
                    Price? currentPrice = await CurrentPriceFromContract(contract, marketDataTypes[0], timeout);
                    result.Add($"{contract.Symbol}\t{currentPrice.Value}\t{contract.Currency}" +
                        $"\t{marketDataTypes[0]}\t{tickTypes[0]}\t{comments[0]}");
                }
                catch (IndexOutOfRangeException ex)
                {
                    result.Add($"{contract.Symbol}\tIndexOutOfRangeException: please try again\t{ex}");
                }
            }

            return result;
        }

        public List<string> RoeFromFundamentalDataList(List<DataStringWithTicker> fundamentalDataList)
        {
            TriggerStatus($"Extracting ROE from the fundamental data list");
            var result = new List<string>();
            // TODO use basic accounts
            result.Add("WARNING! The calculation is not based on the basic accounts! The value is computed by TWS!");

            foreach (DataStringWithTicker fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = XDocumentFromStringWithChecks(fundamentalData.Data, result);
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
                result.Add($"{fundamentalData.Ticker}\t{roeAsString}");
            }

            return result;
        }

        private static string CurrencyFromXDocument(XDocument? xDocument)
        {
            var coGeneralInfoElement = xDocument?.Descendants("CoGeneralInfo").FirstOrDefault();
            var reportingCurrencyElement = coGeneralInfoElement?.Descendants("ReportingCurrency").FirstOrDefault();
            string currency = reportingCurrencyElement?.Attribute("Code")?.Value ?? "NO_CURRENCY";
            return currency;
        }

        public List<string> PayoutRatioYFromFundamentalDataList(List<DataStringWithTicker> fundamentalDataList)
        {
            TriggerStatus($"Extracting Payout Ratio (Y) from the fundamental data list");
            var result = new List<string>();
            result.Add($"Ticker\tNet Income (Y) in M\tDiv. Paid\tPayback Ratio");

            foreach (DataStringWithTicker fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = XDocumentFromStringWithChecks(fundamentalData.Data, result);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }
                IEnumerable<XElement>? statementSection = StatementElementsFromXDocument(xDocument, "AnnualPeriods");

                double netIncome = NetIncome.FromFiscalPeriodElement(statementSection);
                double divPaid = DividendsPaid.FromFiscalPeriodElement(statementSection);
                double paybackRatio = PayoutRatio.FromNetIncomeAndDividends(netIncome, divPaid);

                result.Add($"{fundamentalData.Ticker}\t{netIncome}\t{divPaid}\t{paybackRatio}%");
            }

            return result;
        }


        public List<string> QuarterlyDataFromFundamentalDataList(
            List<DataStringWithTicker> fundamentalDataList,
            Action<List<string>, string, string, XElement?> twiceAYearCalculations,
            Action<List<string>, string, string, XElement?> quarterlyCalculations,
            string statusMessage)
        {
            TriggerStatus(statusMessage);
            List<string> resultQuarterly = new();
            List<string> resultTwiceAYear = new();
            List<string> resultNoFundamentalData = new();

            foreach (DataStringWithTicker fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = XDocumentFromStringWithChecks(fundamentalData.Data, resultNoFundamentalData);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }

                string currency = CurrencyFromXDocument(xDocument);
                var interimPeriodsElement = PeriodsElementFromXDocument(xDocument, "InterimPeriods")?.FirstOrDefault();
                if (interimPeriodsElement == null)
                {
                    if (!resultQuarterly.Any()) resultQuarterly.Add($"Ticker\tCurrency\tNetIncomeQ4(M)\tDivQ4\tRatioQ4\tNetIncomeQ3\tDivQ3\tRatioQ3" +
                        $"\tNetIncomeQ2\tDivQ2\tRatioQ2\tNetIncomeQ1\tDivQ1\tRatioQ1" +
                        $"\tTTM Net Inc\tTTM Div\tTTM Ratio");
                    resultQuarterly.Add($"{fundamentalData.Ticker}\t{currency}" +
                        $"\tInterimPeriods element is null{Enumerable.Repeat("\t", 15).Aggregate((r, n) => r + n)}");
                    continue;
                }

                bool? twiceAYear = ReportingIsTwiceAYear(resultTwiceAYear, xDocument, interimPeriodsElement, fundamentalData.Ticker);

                if (twiceAYear.HasValue)
                {
                    if (twiceAYear.Value)
                    {
                        twiceAYearCalculations(resultTwiceAYear, fundamentalData.Ticker, currency, interimPeriodsElement);
                    }
                    else
                    {
                        quarterlyCalculations(resultQuarterly, fundamentalData.Ticker, currency, interimPeriodsElement);
                    }
                }
            }
            List<string> result = ResultListFromTwoDifferentlyStructuredLists(resultQuarterly, resultTwiceAYear);
            result.AddRange(resultNoFundamentalData);
            return result;
        }

        private bool? ReportingIsTwiceAYear(
            List<string> resultTwiceAYear,
            XDocument? xDocument,
            XElement? interimPeriodsElement,
            string ticker)
        {
            bool? twiceAYear;
            BoolWithError twiceAYearOrError = ReportingFrequencyIsTwiceAYear(interimPeriodsElement);  // otherwise quarterly
            if (twiceAYearOrError.Value == null)
            {
                resultTwiceAYear.Add($"{ticker}\t{twiceAYearOrError.Error}");
            }
            twiceAYear = twiceAYearOrError.Value;
            return twiceAYear;
        }

        public List<string> SharesOutYFromFundamentalDataList(List<DataStringWithTicker> fundamentalDataList)
        {
            TriggerStatus($"Extracting Total Shares Outstanding (Y) from the fundamental data list");
            var result = new List<string>();
            result.Add($"Ticker\tCommon (Y) in M\tPreferred\tTotal");

            foreach (DataStringWithTicker fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = XDocumentFromStringWithChecks(fundamentalData.Data, result);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }

                IEnumerable<XElement>? statementSection = StatementElementsFromXDocument(xDocument, "AnnualPeriods");
                double commonSharesOut = SharesOutstandingFromFiscalPeriodElement(statementSection, "QTCO");
                double preferredSharesOut = SharesOutstandingFromFiscalPeriodElement(statementSection, "QTPO");
                double totalSharesOut = commonSharesOut + preferredSharesOut;

                result.Add($"{fundamentalData.Ticker}\t{commonSharesOut}\t{preferredSharesOut}\t{totalSharesOut}");
            }

            return result;
        }
        public List<string> DesriptionOfCompanyFromFundamentalDataList(List<DataStringWithTicker> fundamentalDataList)
        {
            TriggerStatus($"Extracting business summary from the fundamental data list");
            var result = new List<string>();

            foreach (DataStringWithTicker fundamentalData in fundamentalDataList)
            {
                XDocument? xDocument = XDocumentFromStringWithChecks(fundamentalData.Data, result);
                if (xDocument == null) // some error string has been added
                {
                    continue;
                }

                var textInfoElement = xDocument.Descendants("TextInfo");
                var textElements = textInfoElement?.Descendants("Text");
                var textElementBusinessSummary = textElements?.FirstOrDefault(e => e.Attribute("Type")?.Value == "Business Summary");
                var textElementFinancialSummary = textElements?.FirstOrDefault(e => e.Attribute("Type")?.Value == "Financial Summary");

                result.Add($"{fundamentalData.Ticker}{Environment.NewLine}" +
                    $"{textElementBusinessSummary?.Value}{Environment.NewLine}" +
                    $"{textElementFinancialSummary?.Value}{Environment.NewLine}{string.Concat(Enumerable.Repeat("-", 20))}");
            }

            return result;
        }

        public void PayoutRatioTwiceAYearCalculations(
            List<string> resultTwiceAYear,
            string ticker,
            string currency,
            XElement? interimStatement)
        {
            double netIncomeH1 = NetIncome.FromPeriodsElement(interimStatement, 0);
            double netIncomeH2 = NetIncome.FromPeriodsElement(interimStatement, 1);
            double netIncomeTtm = netIncomeH1 + netIncomeH2;
            double divPaidH1 = DividendsPaid.FromPeriodsElement(interimStatement, 0);
            double divPaidH2 = DividendsPaid.FromPeriodsElement(interimStatement, 1);
            double divPaidTtm = divPaidH1 + divPaidH2;
            double paybackRatioH1 = PayoutRatio.FromNetIncomeAndDividends(netIncomeH1, divPaidH1);
            double paybackRatioH2 = PayoutRatio.FromNetIncomeAndDividends(netIncomeH2, divPaidH2);
            double paybackRatioTtm = PayoutRatio.FromNetIncomeAndDividends(netIncomeTtm, divPaidTtm);

            if (!resultTwiceAYear.Any()) resultTwiceAYear.Add($"Ticker\tCurrency\tNetIncomeH2(M)\tDivH2\tRatioH2\tNetIncomeH1\tDivH1\tRatioH1" +
                $"\tTTM Net Inc\tTTM Div\tTTM Ratio");
            resultTwiceAYear.Add($"{ticker}\t{currency}\t{netIncomeH2}\t{divPaidH2}\t{paybackRatioH2}%\t{netIncomeH1}\t{divPaidH1}\t{paybackRatioH1}%" +
                $"\t{netIncomeTtm}\t{divPaidTtm}\t{paybackRatioTtm}%");
        }

        public void PayoutRatioQuarterlyCalculations(
            List<string> resultQuarterly,
            string ticker,
            string currency,
            XElement? interimStatement)
        {
            double netIncomeQ1 = NetIncome.FromPeriodsElement(interimStatement, 0);
            double netIncomeQ2 = NetIncome.FromPeriodsElement(interimStatement, 1);
            double netIncomeQ3 = NetIncome.FromPeriodsElement(interimStatement, 2);
            double netIncomeQ4 = NetIncome.FromPeriodsElement(interimStatement, 3);
            double netIncomeTtm = netIncomeQ1 + netIncomeQ2 + netIncomeQ3 + netIncomeQ4;
            double divPaidQ1 = DividendsPaid.FromPeriodsElement(interimStatement, 0);
            double divPaidQ2 = DividendsPaid.FromPeriodsElement(interimStatement, 1);
            double divPaidQ3 = DividendsPaid.FromPeriodsElement(interimStatement, 2);
            double divPaidQ4 = DividendsPaid.FromPeriodsElement(interimStatement, 3);
            double divPaidTtm = divPaidQ1 + divPaidQ2 + divPaidQ3 + divPaidQ4;
            double paybackRatioQ1 = PayoutRatio.FromNetIncomeAndDividends(netIncomeQ1, divPaidQ1);
            double paybackRatioQ2 = PayoutRatio.FromNetIncomeAndDividends(netIncomeQ2, divPaidQ2);
            double paybackRatioQ3 = PayoutRatio.FromNetIncomeAndDividends(netIncomeQ3, divPaidQ3);
            double paybackRatioQ4 = PayoutRatio.FromNetIncomeAndDividends(netIncomeQ4, divPaidQ4);
            double paybackRatioTtm = PayoutRatio.FromNetIncomeAndDividends(netIncomeTtm, divPaidTtm);

            if (!resultQuarterly.Any()) resultQuarterly.Add($"Ticker\tCurrency\tNetIncomeQ4(M)\tDivQ4\tRatioQ4\tNetIncomeQ3\tDivQ3\tRatioQ3" +
                $"\tNetIncomeQ2\tDivQ2\tRatioQ2\tNetIncomeQ1\tDivQ1\tRatioQ1" +
                $"\tTTM Net Inc\tTTM Div\tTTM Ratio");
            resultQuarterly.Add($"{ticker}\t{currency}\t{netIncomeQ4}\t{divPaidQ4}\t{paybackRatioQ4}%\t{netIncomeQ3}\t{divPaidQ3}\t{paybackRatioQ3}%" +
                $"\t{netIncomeQ2}\t{divPaidQ2}\t{paybackRatioQ2}%\t{netIncomeQ1}\t{divPaidQ1}\t{paybackRatioQ1}%" +
                $"\t{netIncomeTtm}\t{divPaidTtm}\t{paybackRatioTtm}%");
        }

        /// <summary>
        /// SharesOutTwiceAYearCalculations
        /// </summary>
        /// <param name="resultTwiceAYear"></param>
        /// <param name="ticker"></param>
        /// <param name="periodsElement">AnnualPeriods or InterimPeriods</param>
        public void SharesOutTwiceAYearCalculations(List<string> resultTwiceAYear, string ticker, string currency, XElement? periodsElement)
        {
            if (!resultTwiceAYear.Any()) resultTwiceAYear.Add($"Ticker\tCurrency\tCommon in M\tPreferred\tTotal");

            double commonSharesOutH1 = TotalSharesOutstanding.FromPeriodsElement(periodsElement, "QTCO", 0);
            double preferredSharesOutH1 = TotalSharesOutstanding.FromPeriodsElement(periodsElement, "QTPO", 0);
            double totalSharesOutH1 = commonSharesOutH1 + preferredSharesOutH1;
            // No need for TTM value
            resultTwiceAYear.Add($"{ticker}\t{currency}\t{commonSharesOutH1}\t{preferredSharesOutH1}\t{totalSharesOutH1}");
        }

        /// <summary>
        /// SharesOutQuarterlyCalculations
        /// </summary>
        /// <param name="resultQuarterly"></param>
        /// <param name="ticker"></param>
        /// <param name="periodsElement">AnnualPeriods or InterimPeriods</param>
        public void SharesOutQuarterlyCalculations(List<string> resultQuarterly, string ticker, string currency, XElement? periodsElement)
        {
            // No need for TTM value
            SharesOutTwiceAYearCalculations(resultQuarterly, ticker, currency, periodsElement);
        }

        private XDocument? XDocumentFromStringWithChecks(string stringToParse, List<string> result)
        {
            if (stringToParse.EndsWith(NO_FUNDAMENTAL_DATA))
            {
                result.Add(stringToParse);
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
            Contract contract = ContractFromString(contractString);
            ContractDetails contractDetails = await _ibHost.RequestContractDetailsAsync(contract, timeout);
            return contractDetails;
        }

        private async Task<string> FundamentalDataFromContract(int timeout, string reportType, Contract contract)
        {
            string fundamentalData = await _ibHost.RequestFundamentalDataAsync(contract, reportType, timeout);
            return fundamentalData ?? $"{contract.Symbol}\tNO_FUNDAMENTAL_DATA";
        }

        private async Task<Price> CurrentPriceFromContract(Contract contract, MarketDataType marketDataType, int timeout)
        {
            (var currentPrice, var tickType) = await _ibHost.RequestMarketDataAsync(
                                contract,
                                snapshot: true, 
                                timeout);
            return new Price(currentPrice, (int)marketDataType, (int?)tickType);
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

        private BoolWithError ReportingFrequencyIsTwiceAYear(XElement? interimStatement)
        {
            DateTimeWithError endDate0 = EndDateOfFiscalPeriod(interimStatement, 0);
            DateTimeWithError endDate1 = EndDateOfFiscalPeriod(interimStatement, 1);

            if (endDate0.Value == null)
            {
                return new BoolWithError(null, endDate0.Error);
            }
            if (endDate1.Value == null)
            {
                return new BoolWithError(null, endDate1.Error);
            }

            if (ConditionTwiceAYear(endDate0.Value.Value, endDate1.Value.Value)) // twice a year
            {
                return new BoolWithError(true, "");
            }
            else
            {
                DateTimeWithError endDate2 = EndDateOfFiscalPeriod(interimStatement, 2);
                if (endDate2.Value == null)
                {
                    return new BoolWithError(null, endDate2.Error);
                }

                if (ConditionTwiceAYear(endDate1.Value.Value, endDate2.Value.Value))
                {
                    return new BoolWithError(null, $"Reporting frequency is quarterly, but the third statement does not fit.");
                }

                DateTimeWithError endDate3 = EndDateOfFiscalPeriod(interimStatement, 3);
                if (endDate3.Value == null)
                {
                    return new BoolWithError(null, endDate3.Error);
                }

                if (ConditionTwiceAYear(endDate2.Value.Value, endDate3.Value.Value))
                {
                    return new BoolWithError(null, $"Reporting frequency is quarterly, but the third statement does not fit.");
                }
                return new BoolWithError(false, "");
            }
        }

        private static bool ConditionTwiceAYear(DateTime endDate0, DateTime endDate1)
        {
            return 170 < (endDate0 - endDate1).TotalDays && (endDate0 - endDate1).TotalDays < 200;
        }

        /// <summary>
        /// /// </summary>
        /// <param name="periodsElement"></param>
        /// <param name="periodsAgo">AnnualPeriods or InterimPeriods</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private static DateTimeWithError EndDateOfFiscalPeriod(XElement? periodsElement, int periodsAgo)
        {
            var endDateString = periodsElement?.Descendants("FiscalPeriod").Skip(periodsAgo).FirstOrDefault()?.Attribute("EndDate")?.Value;
            if (!DateTime.TryParse(endDateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                return new DateTimeWithError(null, $"Can not parce the DateTime from {endDateString}");
            }
            return new DateTimeWithError(endDate, "");
        }

        private string? CurrencyFromXmlDocument(XDocument xDocument)
        {
            var reportingCurrencyElement = xDocument?.Descendants("ReportingCurrency").FirstOrDefault();
            return reportingCurrencyElement?.Attribute("Code")?.Value;
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

        private Contract ContractFromString(string contractString)
        {
            var contractArray = contractString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (contractArray.Length == 1)
            {
                return new Contract()
                {
                    Symbol = contractArray[0].Trim(),
                    Currency = IbHost.DEFAULT_CURRENCY,
                    SecType = IbHost.DEFAULT_SEC_TYPE,
                    Exchange = IbHost.DEFAULT_EXCHANGE
                };
            }
            else if (contractArray.Length == 2)
            {
                return new Contract()
                {
                    Symbol = contractArray[0].Trim(),
                    Currency = contractArray[1].Trim(),
                    SecType = IbHost.DEFAULT_SEC_TYPE,
                    Exchange = IbHost.DEFAULT_EXCHANGE
                };
            }
            else if (contractArray.Length == 3)
            {
                return new Contract()
                {
                    Symbol = contractArray[0].Trim(),
                    Currency = contractArray[1].Trim(),
                    SecType = contractArray[2].Trim(),
                    Exchange = IbHost.DEFAULT_EXCHANGE
                };
            }
            else if (contractArray.Length == 4)
            {
                return new Contract()
                {
                    Symbol = contractArray[0].Trim(),
                    Currency = contractArray[1].Trim(),
                    SecType = contractArray[2].Trim(),
                    Exchange = contractArray[3].Trim()
                };
            }
            else
            {
                throw new ApplicationException($"Wrong number of elements in contract's string representation: " +
                    $"{contractArray.Aggregate((r, n) => r + ";" + n)}");
            }
        }
    }
}

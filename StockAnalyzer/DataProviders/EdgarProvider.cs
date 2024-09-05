using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace StockAnalyzer.DataProviders
{
    public class EdgarProvider : IEdgarProvider
    {
        readonly HttpClient _httpClient = new();
        private readonly int REQUEST_DELAY = 200;

        public EdgarProvider()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("StockAnalyzer/1.0 (beltchikov@gmx.de)");
        }

        public async Task<string> Cik(string symbol)
        {
            string response = await _httpClient.GetStringAsync($"https://www.sec.gov/files/company_tickers_exchange.json");
            CompanyTickersExchange companyTickersExchange = JsonSerializer.Deserialize<CompanyTickersExchange>(response) ?? throw new Exception();

            int idx = companyTickersExchange.fields.IndexOf("ticker");
            List<List<object>> symbolData = companyTickersExchange?.data
                .Where(d => (d[idx]?.ToString()?.ToUpper() ?? "") == symbol.Trim().ToUpper())
                .ToList() ?? throw new Exception(); ;
            if (!symbolData.Any())
            {
                return "";
            }

            string cik = symbolData.Single()[0].ToString() ?? throw new Exception();
            return int.Parse(cik).ToString("D10");
        }

        public delegate Task<WithError<IEnumerable<string>>> ConceptFuncDelegate(string symbol, string concept);
        ConceptFuncDelegate IEdgarProvider.CompanyConceptOrError { get => CompanyConceptOrErrorMethod; }

        private async Task<WithError<IEnumerable<string>>> CompanyConceptOrErrorMethod(string symbol, string companyConcept)
        {
            WithError<IEnumerable<string>> resultListOrError = null!;
            List<string> resultList = new();
            string error = "";

            string cik = await Cik(symbol);
            if (string.IsNullOrEmpty(cik))
            {
                error = $"Can not find CIK for symbol {symbol}";
                return new WithError<IEnumerable<string>>(error) { Error = error };
            }

            CompanyConcept companyConceptData = null!;
            try
            {
                string response = await GetResponse10kOr20f(GetUrlsCompanyConcept(cik, companyConcept));
                companyConceptData = JsonSerializer.Deserialize<CompanyConcept>(response) ?? throw new Exception();

                List<CurrencyWithAcronym> distinctCurrencyUnitsWithAcronym = GetCurrencyUnits(companyConceptData).ToList();
                string currency = distinctCurrencyUnitsWithAcronym.First().Acronym;
                List<Currency> distinctCurrencyUnits = distinctCurrencyUnitsWithAcronym.Select(u => u.Currency).ToList();
                distinctCurrencyUnits = HandleMultipleUsdUnitsForFiscalYear(distinctCurrencyUnits);

                List<string> headerColumns = distinctCurrencyUnits.Select(u => u.end).ToList() ?? new List<string>();
                string? header = headerColumns.Aggregate((r, n) => r + "\t" + n);

                List<string> dataList = distinctCurrencyUnits.Select(u => u.val.ToString() ?? "").ToList() ?? new List<string>();
                string? data = dataList.Aggregate((r, n) => r + "\t" + n);

                resultList.AddRange(new List<string>() { header, data, currency });
            }
            catch (Exception ex)
            {
                error = ex.ToString();
            }

            if (resultList.Any())
            {
                resultListOrError = new WithError<IEnumerable<string>>(resultList);
            }
            else
            {
                resultListOrError = new WithError<IEnumerable<string>>(error);
            }

            return resultListOrError;
        }

        public delegate Task<IEnumerable<WithError<string?>>> SimpleBatchProcessingDelegate(
            List<string> symbolList,
            List<string> companyConceptArray);

        /// <summary>
        //IEdgarProvider provider = new EdgarProvider();
        //List<WithError<string?>> result = provider.BatchProcessing(
        //    ["NVDA", "MSFT"],
        //    ["NetIncomeLoss", "ProfitLoss"]).Result.ToList();
        /// </summary>
        SimpleBatchProcessingDelegate IEdgarProvider.SimpleBatchProcessing { get => SimpleBatchProcessingMethod; }

        private async Task<IEnumerable<WithError<string?>>> SimpleBatchProcessingMethod(
          List<string> symbolList,
          List<string> companyConceptList)
        {
            List<SymbolCurrencyDataError> symbolCurrencyDataErrorList = new();
            foreach (var symbol in symbolList)
            {
                SymbolCurrencyDataError symbolCurrencyDataError = new(symbol, "", null, null);
                foreach (string companyConcept in companyConceptList)
                {
                    WithError<IEnumerable<string>> symbolDataOrError = await CompanyConceptOrErrorMethod(symbol, companyConcept);
                    Thread.Sleep(REQUEST_DELAY);
                    if (symbolDataOrError.Data != null)
                    {
                        symbolCurrencyDataError.Currency = symbolDataOrError.Data.Skip(2).First();
                        symbolCurrencyDataError.Data = new(symbolDataOrError.Data.Take(2).ToList());
                        break;
                    }
                    else
                    {
                        symbolCurrencyDataError.Error = symbolDataOrError.Error ?? throw new Exception();
                    }
                }
                if (symbolCurrencyDataError.Data != null && symbolCurrencyDataError.Error != null)
                {
                    symbolCurrencyDataError.Error = null; // We do not care of intermediate errors if get the data finally
                }
                symbolCurrencyDataErrorList.Add(symbolCurrencyDataError);
            }

            List<string> data = TableForMultipleSymbols(symbolCurrencyDataErrorList).ToList();
            string? errors = ErrorsFromSymbolCurrencyDataErrorList(symbolCurrencyDataErrorList);

            List<WithError<string?>> dataWithErrors = data
                .Select(d => new WithError<string?>(d)
                {
                    Data = d,
                    Error = null
                })
                .ToList();
            if (errors != null) dataWithErrors.Add(new WithError<string?>(errors));
            return dataWithErrors;
        }

        public delegate Task<IEnumerable<WithError<string?>>> ComputedBatchProcessingDelegate(
            List<string> symbolList,
            List<string> companyConceptArray1,
            List<string> companyConceptArray2,
            Func<long, long, long> computeFunc, 
            ThreeLabels labels);

        ComputedBatchProcessingDelegate IEdgarProvider.ComputedBatchProcessing { get => ComputedBatchProcessingMethod; }

        private async Task<IEnumerable<WithError<string?>>> ComputedBatchProcessingMethod(
            List<string> symbolList,
            List<string> companyConceptList1,
            List<string> companyConceptList2,
            Func<long, long, long> computeFunc, 
            ThreeLabels labels)
        {
            List<SymbolCurrencyDataError> symbolCurrencyDataErrorList1 = new();
            List<SymbolCurrencyDataError> symbolCurrencyDataErrorList2 = new();

            foreach (var symbol in symbolList)
            {
                SymbolCurrencyDataError symbolCurrencyDataError1 = await SymbolCurrencyDataErrorAsync(symbol, companyConceptList1);
                symbolCurrencyDataErrorList1.Add(symbolCurrencyDataError1);

                SymbolCurrencyDataError symbolCurrencyDataError2 = await SymbolCurrencyDataErrorAsync(symbol, companyConceptList2);
                symbolCurrencyDataErrorList2.Add(symbolCurrencyDataError2);
            }

            List<SymbolDateTwoValues> symbolDateTwoValuesList = SymbolDateTwoValuesList(
                TableForMultipleSymbols(symbolCurrencyDataErrorList1),
                TableForMultipleSymbols(symbolCurrencyDataErrorList2));
            List<string> tableForMultipleSymbolsTwoValues = 
                TableForMultipleSymbols(symbolDateTwoValuesList, computeFunc, labels).ToList();
            string? errors = ErrorsFromSymbolCurrencyDataErrorList(symbolCurrencyDataErrorList1)
                + "\r\n" + ErrorsFromSymbolCurrencyDataErrorList(symbolCurrencyDataErrorList2);

            List<WithError<string?>> dataWithErrors = tableForMultipleSymbolsTwoValues
                .Select(d => new WithError<string?>(d)
                {
                    Data = d,
                    Error = null
                })
                .ToList();
            if (errors != null) dataWithErrors.Add(new WithError<string?>(errors));
            return dataWithErrors;
        }

        private async Task<SymbolCurrencyDataError> SymbolCurrencyDataErrorAsync(string symbol, List<string> companyConceptList1)
        {
            SymbolCurrencyDataError symbolCurrencyDataError1 = new(symbol, "", null, null);
            foreach (string companyConcept in companyConceptList1)
            {
                WithError<IEnumerable<string>> symbolDataOrError = await CompanyConceptOrErrorMethod(symbol, companyConcept);
                Thread.Sleep(REQUEST_DELAY);
                if (symbolDataOrError.Data != null)
                {
                    symbolCurrencyDataError1.Currency = symbolDataOrError.Data.Skip(2).First();
                    symbolCurrencyDataError1.Data = new(symbolDataOrError.Data.Take(2).ToList());
                    break;
                }
                else
                {
                    symbolCurrencyDataError1.Error = symbolDataOrError.Error ?? throw new Exception();
                }
            }

            if (symbolCurrencyDataError1.Data != null && symbolCurrencyDataError1.Error != null)
            {
                symbolCurrencyDataError1.Error = null; // We do not care of intermediate errors if get the data finally
            }

            return symbolCurrencyDataError1;
        }

        public record ThreeLabels (string Label1, string Label2, string Label3);

        private static IEnumerable<string> TableForMultipleSymbols(
            List<SymbolDateTwoValues> symbolDateTwoValuesList,
            Func<long, long, long> computeFunc, 
            ThreeLabels labels)
        {
            List<string> resultList = new();
            List<IGrouping<string, SymbolDateTwoValues>> groupedBySymbol = symbolDateTwoValuesList
                            .GroupBy(d => d.Symbol)
                            .ToList();

            foreach (IGrouping<string, SymbolDateTwoValues> group in groupedBySymbol)
            {
                string datesLine = group.Select(g => g.Date).Reverse().Select(l => l.ToString("yyyy-MM-dd"))
                    .Aggregate((r, n) => r + "\t" + n);
                string header = $"{group.Key}\t{datesLine}";

                List<long> values1List = group.Select(g => g.Value1).Reverse().ToList();
                List<long> values2List = group.Select(g => g.Value2).Reverse().ToList();
                string values1Line = $"{labels.Label1}\t" + values1List.Select(v1 => v1.ToString()).Aggregate((r, n) => r + "\t" + n);
                string values2Line = $"{labels.Label2}\t" + values2List.Select(v2 => v2.ToString()).Aggregate((r, n) => r + "\t" + n);
                string computedValuesLine = $"{labels.Label3}\t" + values1List
                    .Zip(values2List, (v1, v2) => computeFunc(v1, v2))
                    .Select(cv => cv.ToString())
                    .Aggregate((r, n) => r + "\t" + n);

                resultList.AddRange(new List<string>
                    { header, values1Line, values2Line, computedValuesLine, Environment.NewLine });
            }

            return resultList;
        }

        private static List<SymbolDateTwoValues> SymbolDateTwoValuesList(
            IEnumerable<string> tableForMultipleSymbols1,
            IEnumerable<string> tableForMultipleSymbols2)
        {
            List<SymbolDateTwoValues> resultList = new();

            string header1 = tableForMultipleSymbols1.First();
            string header2 = tableForMultipleSymbols2.First();
            List<string> header1AsList = header1.Split("\t", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            List<string> header2AsList = header2.Split("\t", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            List<string> dates1AsStringList = header1AsList.Skip(1).ToList();
            List<string> dates2AsStringList = header2AsList.Skip(1).ToList();
            List<DateOnly> dates1 = dates1AsStringList.Select(d => d.ToDateOnly()).ToList();
            List<DateOnly> dates2 = dates2AsStringList.Select(d => d.ToDateOnly()).ToList();

            List<string> symbolsAndDataStringsWithoutHeader1 = tableForMultipleSymbols1.Skip(1).ToList();
            List<string> symbolsAndDataStringsWithoutHeader2 = tableForMultipleSymbols2.Skip(1).ToList();
            List<List<string>> symbolsAndDataWithoutHeader1 = symbolsAndDataStringsWithoutHeader1
                .Select(t => t.Split("\t", StringSplitOptions.TrimEntries).ToList())
                .ToList();
            List<List<string>> symbolsAndDataWithoutHeader2 = symbolsAndDataStringsWithoutHeader2
                .Select(t => t.Split("\t", StringSplitOptions.TrimEntries).ToList())
                .ToList();
            List<List<string>> dataForAllSymbols1 = symbolsAndDataWithoutHeader1.Select(s => s.Skip(1).ToList()).ToList();
            List<List<string>> dataForAllSymbols2 = symbolsAndDataWithoutHeader2.Select(s => s.Skip(1).ToList()).ToList();

            List<string> symbols = symbolsAndDataWithoutHeader1.Select(s => s.First()).ToList();

            for (int i = 0; i < symbols.Count; i++)
            {
                string symbol = symbols[i];
                List<string> data1ForSymbol = dataForAllSymbols1[i];
                List<string> data2ForSymbol = dataForAllSymbols2[i];

                for (int ii = dates1.Count - 1; ii >= 0; ii--)
                {
                    DateOnly date1 = dates1[ii];
                    string data1 = data1ForSymbol[ii];
                    if (string.IsNullOrWhiteSpace(data1))
                        continue;

                    int idxOfDateInData2 = dates2.IndexOf(date1);
                    if (idxOfDateInData2 >= 0)
                    {
                        string data2 = data2ForSymbol[idxOfDateInData2];
                        if (!string.IsNullOrWhiteSpace(data2))
                        {
                            long data1AsLong = Convert.ToInt64(data1);
                            long data2AsLong = Convert.ToInt64(data2);

                            SymbolDateTwoValues symbolDateTwoValues = new(symbol, date1, data1AsLong, data2AsLong);
                            resultList.Add(symbolDateTwoValues);
                        }
                    }
                }
            }

            return resultList;
        }

        private static string? ErrorsFromSymbolCurrencyDataErrorList(List<SymbolCurrencyDataError> symbolCurrencyDataErrorList)
        {
            if (!symbolCurrencyDataErrorList.Any()) return null;

            List<SymbolCurrencyDataError> symbolCurrencyDataErrorListWithErrors = symbolCurrencyDataErrorList
                            .Where(d => d.Error != null)
                            .ToList();
            if (!symbolCurrencyDataErrorListWithErrors.Any()) return null;

            return symbolCurrencyDataErrorListWithErrors
                .Select(l => l.Error)
                .Aggregate((r, n) => r + "\r\n" + n);
        }

        private static List<string> TableForMultipleSymbols(List<SymbolCurrencyDataError> symbolCurrencyDataErrorList)
        {
            List<SymbolCurrencyDataError> resultListData = symbolCurrencyDataErrorList.Where(d => d.Data != null).ToList();

            List<string> symbolsWithDataList = resultListData.Select(l => l.Symbol).ToList();
            List<string> currencies = resultListData.Select(l => l.Currency).ToList();
            List<List<string>> symbolDataList = new();
            symbolDataList.AddRange(from List<string>? dataList in resultListData.Select(l => l.Data)
                                    where dataList != null
                                    select dataList);
            return TableForMultipleSymbols(symbolsWithDataList, currencies, symbolDataList).ToList();
        }

        private static UrlsCompanyConcept GetUrlsCompanyConcept(string cik, string companyConcept)
        {
            return new UrlsCompanyConcept(
                $"https://data.sec.gov/api/xbrl/companyconcept/CIK{cik}/us-gaap/{companyConcept}.json",
                $"https://data.sec.gov/api/xbrl/companyconcept/CIK{cik}/ifrs-full/{companyConcept}.json");
        }

        private static IEnumerable<CurrencyWithAcronym> GetCurrencyUnits(CompanyConcept companyConceptData)
        {
            List<Currency> currencyUnitList = null!;
            List<CurrencyWithAcronym> currencyWithAcronymList = null!;

            if (companyConceptData.units.USD != null)
            {
                currencyUnitList = companyConceptData.units.USD;
                currencyWithAcronymList = currencyUnitList.Select(c => new CurrencyWithAcronym(c, "USD")).ToList();
            }
            else if (companyConceptData.units.EUR != null)
            {
                currencyUnitList = companyConceptData.units.EUR;
                currencyWithAcronymList = currencyUnitList.Select(c => new CurrencyWithAcronym(c, "EUR")).ToList();
            }
            else
            {
                throw new ApplicationException($"Currency not found for {companyConceptData.entityName}");
            }

            return currencyWithAcronymList
                .Where(u => u.Currency.form == "10-K" || u.Currency.form == "20-F")
                .DistinctBy(u => u.Currency.end);
        }

        private async Task<string> GetResponse10kOr20f(UrlsCompanyConcept urls)
        {
            try
            {
                return await _httpClient.GetStringAsync(urls.Url10k);
            }
            catch (HttpRequestException)
            {
                return await _httpClient.GetStringAsync(urls.Url20f);
            }

        }

        private List<Currency> HandleMultipleUsdUnitsForFiscalYear(List<Currency> distinctUsdUnitsTill2019)
        {
            List<Currency> resultList = new();

            List<IGrouping<int, Currency>> unitsGroupedByFiscalYear = distinctUsdUnitsTill2019
                .GroupBy(u => u.fy)
                .ToList();
            foreach (IGrouping<int, Currency> group in unitsGroupedByFiscalYear)
            {
                if (group.Count() == 1)
                {
                    resultList.Add(group.First());
                }
                else
                {
                    TimeSpan minTimeSpan = group
                        .Select(u => u.FiledToEndDiff())
                        .MinBy(s => s.TotalMilliseconds);
                    List<Currency> unitWithFiledClosestToEndList = group
                        .Where(g => g.FiledToEndDiff() == minTimeSpan)
                        .ToList();

                    if (unitWithFiledClosestToEndList.Count != 1) throw new ApplicationException("Unexpected!");
                    resultList.Add(unitWithFiledClosestToEndList.First());
                }
            }

            return resultList;
        }

        public IEnumerable<string> InterpolateDataForMissingDates(List<string> data)
        {
            List<DateOnly> dates = data[0]
                .Split("\t")
                .Skip(1)
                .Select(s => DateOnly.ParseExact(s, "yyyy-MM-dd")).ToList();

            List<string> symbols = data
                .Skip(1)
                .Select(r => r.Split("\t").ToList())
                .Select(v => v[0])
                .ToList();

            List<List<long?>> values = data
                .Skip(1)
                .Select(r => r.Split("\t").ToList())
                .Select(v => v.Skip(1).ToList())
                .Select(r => r.Select(v => (long?)(string.IsNullOrWhiteSpace(v) ? null : long.Parse(v))).ToList())
                .ToList();

            List<Earning> earnings = CreateEarningsList(dates, values);
            List<Earning> earningsWithInterpolatedValues = InterpolateMissingValues(earnings);
            List<string> resultList = ListOfStringsFromEarnings(earningsWithInterpolatedValues, symbols);

            return resultList;
        }

        private static List<string> ListOfStringsFromEarnings(List<Earning> earnings, List<string> symbols)
        {
            List<string> resultList = new();

            string resultHeader = "Symbol\t" + earnings.Select(r => r.Date)
                .Select(y => y.ToString("yyyy-MM-dd"))
                .Aggregate((r, n) => r + "\t" + n);
            resultList.Add(resultHeader);

            for (int i = 0; i < symbols.Count; i++)
            {
                string resultDataRow = symbols[i] + "\t" + earnings
                    .Select(e => e.Data[i])
                    .Select(n => n.HasValue ? n.ToString() : "")
                    .Select(x => string.IsNullOrWhiteSpace(x) ? "" : x)
                    .Aggregate((r, n) => r + "\t" + n);
                resultList.Add(resultDataRow);
            }

            return resultList;
        }

        private static List<Earning> InterpolateMissingValues(List<Earning> earnings)
        {
            List<Earning> earningsWithInterpolatedValues = new();

            for (int eIdx = 0; eIdx < earnings.Count; eIdx++)
            {
                Earning earning = earnings[eIdx];
                Earning earningWithInterpolatedValues = null!;

                for (int i = 0; i < earning.Data.Count; i++)
                {
                    long? value = earning.Data[i];
                    if (!value.HasValue)
                    {
                        List<Earning> earningBefore = earnings.Take(eIdx).ToList();
                        List<Earning> earningAfter = earnings.Skip(eIdx + 1).ToList();

                        if (!(earningBefore.Any(e => e.Data[i].HasValue) && earningAfter.Any(e => e.Data[i].HasValue)))
                        {
                            continue;
                        }

                        long? valueBefore = earningBefore.Select(e => e.Data[i]).Last(v => v.HasValue);
                        long? valueAfter = earningAfter.Select(e => e.Data[i]).First(v => v.HasValue);

                        List<long?> dataWithInterpolatedValues = new(earning.Data);
                        if (valueBefore.HasValue && valueAfter.HasValue)
                        {
                            dataWithInterpolatedValues[i] = (valueBefore.Value + valueAfter.Value) / 2;

                        }
                        earningWithInterpolatedValues = new Earning(earning.Date, dataWithInterpolatedValues);
                        earningsWithInterpolatedValues.AddOrMerge(earningWithInterpolatedValues);
                    }
                }

                if (earningWithInterpolatedValues is null)
                {
                    earningsWithInterpolatedValues.AddOrMerge(earning);
                }

            }

            return earningsWithInterpolatedValues;
        }

        private static List<Earning> CreateEarningsList(List<DateOnly> datesList, List<List<long?>> intValuesList)
        {
            List<Earning> earnings = new();
            for (int i = 0; i < datesList.Count; i++)
            {
                DateOnly date = datesList[i];

                List<long?> valuesForDate = new();
                foreach (List<long?> valuesRow in intValuesList)
                {
                    long? value = valuesRow[i];
                    valuesForDate.Add(value);
                }
                Earning earning = new(date, valuesForDate);
                earnings.Add(earning);
            }

            return earnings;
        }

        private static IEnumerable<string> TableForMultipleSymbols(
              List<string> symbols,
              List<string> currencies,
              List<List<string>> symbolDataList)
        {
            List<string> uniqueDatesStringsListSorted = symbolDataList
                .Where(s => s.Any())
                .Select(d => d[0])
                .SelectMany(u => u.Split("\t"))
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            List<string> resultList = new() { "Symbol\t" + uniqueDatesStringsListSorted.Aggregate((r, n) => r + "\t" + n) };

            string dataRow = "";
            for (int i = 0; i < symbols.Count; i++)
            {
                string symbol = symbols[i];
                string currency = currencies[i];
                dataRow = FirstDataRowCell(symbol, currency);

                List<List<string>> symbolData = symbolDataList[i].Select(l => l.Split("\t").ToList()).ToList();
                foreach (var date in uniqueDatesStringsListSorted)
                {
                    if (!symbolData.Any()) continue;

                    int ii = symbolData.First().IndexOf(date);
                    string data = ii >= 0 ? symbolData.Last()[ii] : null!;
                    dataRow += ("\t" + data);
                }

                resultList.Add(dataRow);
            }

            return resultList;
        }

        private static string FirstDataRowCell(string symbol, string currency)
        {
            return $"{symbol} ({currency})";
        }

        public IEnumerable<string> Cagr(List<string> inputList, int periods)
        {
            List<string> resultList = new List<string>();
            if (!inputList.Any()) return resultList;
            resultList.Add("Symbol\tYears\tGrowth\tCAGR");

            string datesString = inputList[0];
            if (string.IsNullOrWhiteSpace(datesString)) throw new ApplicationException();

            List<string> datesStringList = datesString.IntelliSplit().ToList();
            if (!datesStringList.Any()) throw new ApplicationException();
            datesStringList = datesStringList.Skip(1).ToList();

            List<DateOnly> datesList = datesStringList.Select(s => s.ToDateOnly()).ToList();
            if (!datesStringList.Any()) throw new ApplicationException();

            DateOnly lastDate = datesList.Last();
            int lastYear = lastDate.Year;
            int firstYear = lastYear - periods;

            List<DateOnly> datesListPeriods = datesList.Where(d => d.Year >= firstYear).ToList();
            if (!datesStringList.Any()) throw new ApplicationException();

            List<string> symbolsList = inputList.Skip(1).Select(l => l.IntelliSplit().ToList()[0]).ToList();
            for (int i = 0; i < symbolsList.Count; i++)
            {
                string symbol = symbolsList[i];
                List<string> symbolDataListAsString = inputList[i + 1].IntelliSplit().Skip(1).ToList();
                symbolDataListAsString = symbolDataListAsString.Skip(symbolDataListAsString.Count - datesListPeriods.Count).ToList();

                int idxFirst = FirstIndexOfNotEmptyString(symbolDataListAsString);
                int idxLast = LastIndexOfNotEmptyString(symbolDataListAsString);
                if (idxFirst < 0 || idxLast < 0) continue;

                long firstData = Convert.ToInt64(symbolDataListAsString[idxFirst]);
                long lastData = Convert.ToInt64(symbolDataListAsString[idxLast]);

                int years = datesListPeriods[idxLast].Year - datesListPeriods[idxFirst].Year + 1;
                double growth = CalculateGrowth(lastData, firstData);
                double cagr = Math.Round(Math.Pow(growth, 1 / (double)years) - 1, 3);

                resultList.Add($"{symbol}\t{years}\t{growth}\t{cagr}");
            }

            return resultList;
        }

        private static double CalculateGrowth(long lastData, long firstData)
        {
            if (firstData < 0 && lastData >= 0)
            {
                long correctionValue = 1 - firstData;
                long firstDataCorrected = firstData + correctionValue;
                long lastDataCorrected = lastData + correctionValue;
                return Math.Round(lastDataCorrected / (double)firstDataCorrected, 3);
            }
            else
            {
                return lastData < 0 && firstData < 0 ? 0 : Math.Round(lastData / (double)firstData, 3);
            }
        }

        private static int LastIndexOfNotEmptyString(List<string> symbolDataListAsString)
        {
            for (int i = symbolDataListAsString.Count - 1; i >= 0; i--)
            {
                if (!string.IsNullOrWhiteSpace(symbolDataListAsString[i]))
                    return i;
            }
            return -1;
        }

        private static int FirstIndexOfNotEmptyString(List<string> symbolDataListAsString)
        {
            for (int i = 0; i < symbolDataListAsString.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(symbolDataListAsString[i]))
                    return i;
            }
            return -1;
        }

        public IEnumerable<string> MergeMultipleTables(List<string> inputList)
        {
            inputList = RemoveNewLinesEnd(inputList);
            inputList = RemoveNewLinesStart(inputList);

            if (!inputList.Contains(Environment.NewLine))
            {
                MessageBox.Show("No multiple table found");
                return inputList;  
            }
            
            List<SymbolCurrencyDataError> symbolCurrencyDataErrorList = new();



            return TableForMultipleSymbols(symbolCurrencyDataErrorList);
        }

        private static List<string> RemoveNewLinesStart(List<string> inputList)
        {
            List<string> resultList = new();

            for(int i = 0; i < inputList.Count; i++)
            {
                if (inputList[i] == Environment.NewLine)
                    continue;
                resultList.Add(inputList[i]);
            }

            return resultList;
        }

        private static List<string> RemoveNewLinesEnd(List<string> inputList)
        {
            List<string> resultList = new();

            for (int i = inputList.Count-1; i >=0; i--)
            {
                if (inputList[i] == Environment.NewLine)
                    continue;
                resultList.Insert(0,inputList[i]);   
            }

            return resultList;  
        }
    }

    internal record SymbolDateTwoValues(string Symbol, DateOnly Date, long Value1, long Value2);

    public class WithError<T>
    {
        public WithError(T? data)
        {
            Data = data;
            Error = null;
        }

        public WithError(string error)
        {
            Error = error;
            Data = default;
        }

        public T? Data { get; set; }
        public string? Error { get; set; }

        public bool HasData => Data != null;
        public bool HasError => !string.IsNullOrWhiteSpace(Error);
    }

    #region Edgar API autogenerated

    /// <summary>
    /// Autogenerated from https://www.sec.gov/files/company_tickers_exchange.json
    /// </summary>
    public class CompanyTickersExchange
    {
        public List<string> fields { get; set; }
        public List<List<object>> data { get; set; }
    }

    /// <summary>
    /// Autogenerated from https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/LongTermDebt.json
    /// </summary>
    public class CompanyConcept
    {
        public int cik { get; set; }
        public string taxonomy { get; set; }
        public string tag { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public string entityName { get; set; }
        public Units units { get; set; }
    }

    public class Currency
    {
        public string end { get; set; }
        public object val { get; set; }
        public string accn { get; set; }
        public int fy { get; set; }
        public string fp { get; set; }
        public string form { get; set; }
        public string filed { get; set; }
        public string frame { get; set; }
    }

    public class Units
    {
        public List<Currency> USD { get; set; }
        public List<Currency> EUR { get; set; }
    }

    #endregion

    internal static class EdgarApiExtensions
    {
        public static TimeSpan FiledToEndDiff(this Currency currency)
        {
            return DateTime.ParseExact(currency.filed, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                            - DateTime.ParseExact(currency.end, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }

    internal class CurrencyWithAcronym
    {
        public CurrencyWithAcronym(Currency currency, string acronym)
        {
            Currency = currency;
            Acronym = acronym;
        }

        public Currency Currency { get; set; }
        public string Acronym { get; set; }
    }


    internal class SymbolCurrencyDataError
    {
        public string Symbol { get; set; }
        public string Currency { get; set; }
        public List<string>? Data { get; set; }
        public string? Error { get; set; }


        public SymbolCurrencyDataError(string symbol, string currency, List<string>? data, string? error)
        {
            Symbol = symbol;
            Currency = currency;
            Data = data;
            Error = error;
        }
    }

    internal record UrlsCompanyConcept(string Url10k, string Url20f);

    internal record Earning(DateOnly Date, List<long?> Data);

    internal static class EdgarProviderExtensions
    {
        public static DateOnly ToDateOnly(this string stringToConvert)
        {
            try
            {
                return DateOnly.ParseExact(stringToConvert, "yyyy-MM-dd");
            }
            catch (FormatException)
            {

                return DateOnly.ParseExact(stringToConvert, "dd.MM.yyyy");
            }

            throw new NotImplementedException();
        }


        public static IEnumerable<string> IntelliSplit(this string stringToSplit)
        {
            IEnumerable<string> resultList = stringToSplit.Split("\t", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (resultList.Count() > 1)
            {
                return resultList;
            }

            resultList = stringToSplit.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (resultList.Count() > 1)
            {
                return resultList;
            }

            throw new NotImplementedException();
        }

        public static void AddOrMerge(this List<Earning> earningsList, Earning earning)
        {
            Earning? earningToUpdate = earningsList.FirstOrDefault(e => e.Date == earning.Date);
            int idx = -1;
            if (earningToUpdate == null)
            {
                earningsList.Add(earning);
            }
            else
            {
                List<long?> updatedDataList = new(earningToUpdate.Data);

                for (int i = 0; i < updatedDataList.Count; i++)
                {
                    long? value = updatedDataList[i];
                    if (!value.HasValue)
                    {
                        long? incomingValue = earning.Data[i];
                        if (incomingValue.HasValue)
                        {
                            updatedDataList[i] = incomingValue;
                        }
                    }
                }

                idx = earningsList.IndexOf(earningToUpdate);
                earningToUpdate = earningToUpdate with { Data = updatedDataList };
                earningsList[idx] = earningToUpdate;
            }
        }
    }
}

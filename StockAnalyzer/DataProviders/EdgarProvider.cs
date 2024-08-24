using Microsoft.CodeAnalysis;
using StockAnalyzer.DataProviders.Types;
using StockAnalyzer.DataProviders.Types.EdgarApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public class EdgarProvider : IEdgarProvider
    {
        readonly HttpClient _httpClient = new();

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
        ConceptFuncDelegate IEdgarProvider.CompanyConceptOrError{ get => CompanyConceptOrErrorMethod; }

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

        public delegate Task<IEnumerable<WithError<string?>>> BatchProcessingDelegate(
            List<string> symbolList,
            List<string> companyConceptArray,
            ConceptFuncDelegate processingFunc);

        BatchProcessingDelegate IEdgarProvider.BatchProcessing { get => BatchProcessingMethod;}

        private static async Task<IEnumerable<WithError<string?>>> BatchProcessingMethod(
          List<string> symbolList,
          List<string> companyConceptList,
          ConceptFuncDelegate processingFunc)
        {
            List<SymbolCurrencyDataError> symbolCurrencyDataErrorList = new();
            foreach (var symbol in symbolList)
            {
                SymbolCurrencyDataError symbolCurrencyDataError = new(symbol, "", null, null);
                foreach (string companyConcept in companyConceptList)
                {
                    WithError<IEnumerable<string>> symbolDataOrError = await processingFunc(symbol, companyConcept);
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

using Microsoft.CodeAnalysis;
using StockAnalyzer.DataProviders.Types;
using StockAnalyzer.DataProviders.Types.UsGaap;
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
            string url = $"https://www.sec.gov/files/company_tickers_exchange.json";
            var response = await _httpClient.GetStringAsync(url);
            CompanyTickersExchange companyTickersExchange = JsonSerializer.Deserialize<CompanyTickersExchange>(response) ?? throw new Exception();

            List<object> symbolData = companyTickersExchange?.data
                .Where(d => (d[2]?.ToString() ?? "") == symbol.Trim().ToUpper())
                .Single() ?? throw new Exception();

            string cik = symbolData[0].ToString() ?? throw new Exception();
            cik = int.Parse(cik).ToString("D10");
            return cik;
        }

        public async Task<IEnumerable<string>> BatchProcessing(
            List<string> symbolList,
            string companyConcept,
            Func<string, string, Task<IEnumerable<string>>> processingFunc)
        {
            List<List<string>> symbolDataList = new();
            foreach (var symbol in symbolList)
            {
                symbolDataList.Add((await processingFunc(symbol, companyConcept)).ToList());
            }
            
            return TableForMultipleSymbols(symbolList, symbolDataList).ToList();
        }

        public async Task<IEnumerable<string>> CompanyConcept(string symbol, string companyConcept)
        {
            string url = $"https://data.sec.gov/api/xbrl/companyconcept/CIK{await Cik(symbol)}/us-gaap/{companyConcept}.json";
            var response = await _httpClient.GetStringAsync(url);
            
            var stockholdersEquity = JsonSerializer.Deserialize<CompanyConcept>(response) ?? throw new Exception();
            List<USD> distinctUsdUnits = stockholdersEquity.units.USD.DistinctBy(u => u.end).ToList();

            List<string> headers = distinctUsdUnits.Select(u => u.end).ToList() ?? new List<string>();
            var header = $"Errors\t" + headers.Aggregate((r, n) => r + "\t" + n);

            List<string> dataList = distinctUsdUnits.Select(u => u.val.ToString() ?? "").ToList() ?? new List<string>();
            var data = $"\t" + dataList.Aggregate((r, n) => r + "\t" + n);

            return new List<string>() { header, data };
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

        private static IEnumerable<string> TableForMultipleSymbols(List<string> symbols, List<List<string>> symbolDataList)
        {
            List<string> uniqueDatesStringsListSorted = symbolDataList
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
                dataRow = symbol;
                List<List<string>> symbolData = symbolDataList[i].Select(l => l.Split("\t").ToList()).ToList();

                foreach (var date in uniqueDatesStringsListSorted)
                {
                    int ii = symbolData.First().IndexOf(date);
                    string data = ii >= 0 ? symbolData.Last()[ii] : null!;
                    dataRow += ("\t" + data);
                }

                resultList.Add(dataRow);
            }

            return resultList;
        }
    }

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

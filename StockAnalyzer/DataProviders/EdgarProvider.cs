using StockAnalyzer.DataProviders.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public class EdgarProvider : EdgarProviderBase, IEdgarProvider
    {
        record Earning(DateOnly Date, List<int?> Data);

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

        public async Task<IEnumerable<string>> StockholdersEquity(List<string> symbolList)
        {
            List<List<string>> symbolDataList = new();
            foreach (var symbol in symbolList)
            {
                symbolDataList.Add((await StockholdersEquity(symbol)).ToList());
            }

            // Test
            symbolList = new List<string> { "A", "B" };
            symbolDataList = new List<List<string>> {
                new List<string>{"2024-01-01\t2024-03-01\t2024-04-01", "10\t30\t40" },
                new List<string>{"2024-01-01\t2024-02-01\t2024-04-01", "100\t200\t400" }
            };

            return TableForMultipleSymbols(symbolList, symbolDataList).ToList();
        }

        public async Task<IEnumerable<string>> StockholdersEquity(string symbol)
        {
            string cik = await Cik(symbol);
            string url = $"https://data.sec.gov/api/xbrl/companyconcept/CIK{cik}/us-gaap/LiabilitiesAndStockholdersEquity.json";
            var response = await _httpClient.GetStringAsync(url);

            var liabilitiesAndStockholdersEquity = JsonSerializer.Deserialize<LiabilitiesAndStockholdersEquity>(response) ?? throw new Exception();
            List<USD> distinctUsdUnits = liabilitiesAndStockholdersEquity.units.USD.DistinctBy(u => u.end).ToList();

            List<string> headers = distinctUsdUnits.Select(u => u.end).ToList() ?? new List<string>();
            var header = headers.Aggregate((r, n) => r + "\t" + n);

            List<string> dataList = distinctUsdUnits.Select(u => u.val.ToString() ?? "").ToList() ?? new List<string>();
            var data = dataList.Aggregate((r, n) => r + "\t" + n);

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
            
            List<List<int?>> values = data
                .Skip(1)
                .Select(r => r.Split("\t").ToList())
                .Select(v => v.Skip(1).ToList())
                .Select(r => r.Select(v => (int?)(string.IsNullOrWhiteSpace(v) ? null : int.Parse(v))).ToList())
                .ToList();

            List<Earning> earnings = CreateEarningsList(dates, values);
            List<Earning> earningsWithInterpolatedValues = InterpolateMissingValues(earnings);
            List<string> resultList = ListOfStringsFromEarnings(earningsWithInterpolatedValues, symbols);
            
            return resultList;
        }

        private List<string> ListOfStringsFromEarnings(List<Earning> earnings, List<string> symbols)
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

        private List<Earning> InterpolateMissingValues(List<Earning> earnings)
        {
            List<Earning> earningsWithInterpolatedValues = new();

            for (int eIdx = 0; eIdx < earnings.Count; eIdx++)
            {
                Earning earning = earnings[eIdx];
                Earning earningWithInterpolatedValues = null!;

                for (int i = 0; i < earning.Data.Count; i++)
                {
                    int? value = earning.Data[i];
                    if (!value.HasValue)
                    {
                        List<Earning> earningBefore = earnings.Take(eIdx).ToList();
                        List<Earning> earningAfter = earnings.Skip(eIdx + 1).ToList();

                        int? valueBefore = earningBefore.Select(e => e.Data[i]).Last(v => v.HasValue);
                        int? valueAfter = earningAfter.Select(e => e.Data[i]).First(v => v.HasValue);

                        List<int?> dataWithInterpolatedValues = new(earning.Data);
                        if (valueBefore.HasValue && valueAfter.HasValue)
                        {
                            dataWithInterpolatedValues[i] = (valueBefore.Value + valueAfter.Value) / 2;

                        }
                        earningWithInterpolatedValues = new Earning(earning.Date, dataWithInterpolatedValues);
                        earningsWithInterpolatedValues.Add(earningWithInterpolatedValues);
                    }
                }

                if (earningWithInterpolatedValues is null)
                {
                    earningsWithInterpolatedValues.Add(new Earning(earning.Date, earning.Data));
                }

            }

            return earningsWithInterpolatedValues;
        }

        private static List<Earning> CreateEarningsList(List<DateOnly> datesList, List<List<int?>> intValuesList)
        {
            List<Earning> earnings = new();
            for (int i = 0; i < datesList.Count; i++)
            {
                DateOnly date = datesList[i];

                List<int?> valuesForDate = new();
                foreach (List<int?> valuesRow in intValuesList)
                {
                    int? value = valuesRow[i];
                    valuesForDate.Add(value);
                }
                Earning earning = new(date, valuesForDate);
                earnings.Add(earning);
            }

            return earnings;
        }
    }
}

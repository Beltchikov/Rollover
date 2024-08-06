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
            throw new NotImplementedException();
        }
    }
}

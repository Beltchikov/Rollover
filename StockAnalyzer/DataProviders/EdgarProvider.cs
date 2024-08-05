using StockAnalyzer.DataProviders.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    internal class EdgarProvider : EdgarProviderBase, IEdgarProvider
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
            cik = (Int32.Parse(cik)).ToString("D10");
            return cik;
        }

        public async Task<IEnumerable<string>> StockholdersEquity(List<string> symbolList)
        {
            //return await Task.Run(() =>
            //{
            //    return new List<string>() {
            //    "Symbol\t30.06.2023\t02.07.2023\t30.09.2023\t01.10.2023\t31.12.2023\t31.03.2024\t30.06.2024",
            //    "JNJ\t\t1,91686E+11\t\t1,66061E+11\t1,67558E+11\t1,71966E+11\t1,81088E+11",
            //    "PG\t1,20829E+11\t\t1,22531E+11\t\t1,20709E+11\t1,19598E+11\t"
            //};
            //});

            List<List<string>> symbolDataList = new();
            foreach (var symbol in symbolList)
            {
                symbolDataList.Add((await StockholdersEquity(symbol)).ToList());
            }

            List<string> resultList = TableForMultipleSymbols(symbolList, symbolDataList).ToList();
            return resultList;
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
    }
}

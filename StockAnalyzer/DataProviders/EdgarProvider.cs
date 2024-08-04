using StockAnalyzer.DataProviders.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    internal class EdgarProvider : IEdgarProvider
    {
        readonly HttpClient _httpClient = new();

        public EdgarProvider()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("StockAnalyzer/1.0 (beltchikov@gmx.de)");
        }


        public async Task<IEnumerable<string>> StockholdersEquity(string symbol)
        {
            string cik = "0000200406"; // TODO CIK Repository  
            string url = $"https://data.sec.gov/api/xbrl/companyconcept/CIK{cik}/us-gaap/LiabilitiesAndStockholdersEquity.json";
            var response = await _httpClient.GetStringAsync(url);

            var liabilitiesAndStockholdersEquity = JsonSerializer.Deserialize<LiabilitiesAndStockholdersEquity>(response) ?? throw new Exception();
            List<string> headers = liabilitiesAndStockholdersEquity?.units.USD.Select(u => u.end).ToList() ?? new List<string>();
            var header = headers.Aggregate((r, n) => r + "\t" + n);

            List<string> dataList = liabilitiesAndStockholdersEquity?.units.USD.Select(u => u.val.ToString() ?? "").ToList() ?? new List<string>();
            var data = dataList.Aggregate((r, n) => r + "\t" + n);

            return new List<string>() { header, data };
        }
    }
}

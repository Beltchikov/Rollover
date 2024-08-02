using StockAnalyzer.DataProviders.Types;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    internal class EdgarProvider : IEdgarProvider
    {
        HttpClient _httpClient = new();

        public EdgarProvider()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("StockAnalyzer/1.0 (beltchikov@gmx.de)");
        }


        public async Task<IEnumerable<string>> StockholdersEquity(string symbol)
        {
            string cik = "0000200406"; // TODO
            string url = $"https://data.sec.gov/api/xbrl/companyconcept/CIK{cik}/us-gaap/LiabilitiesAndStockholdersEquity.json";
            var response = await _httpClient.GetStringAsync(url);

            var liabilitiesAndStockholdersEquity = JsonSerializer.Deserialize<LiabilitiesAndStockholdersEquity>(response);


            // TODO


            return new List<string>() {
                "Symbol\t2024-03-31\t2024-06-30",
                "JNJ\t171966000000\t181088000000",
                "PG\t10\t18"};

        }
    }
}
    
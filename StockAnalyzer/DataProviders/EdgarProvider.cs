using StockAnalyzer.DataProviders.Types;
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
        HttpClient _httpClient = new();

        public EdgarProvider()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("StockAnalyzer/1.0 (beltchikov@gmx.de)");
        }


        [Obsolete]
        public async Task<IEnumerable<string>> StockholdersEquityOld(string symbol)
        {
            string cik = "0000200406"; // TODO
            string url = $"https://data.sec.gov/api/xbrl/companyconcept/CIK{cik}/us-gaap/LiabilitiesAndStockholdersEquity.json";
            var response = await _httpClient.GetStringAsync(url);

            var liabilitiesAndStockholdersEquity = JsonSerializer.Deserialize<LiabilitiesAndStockholdersEquity>(response);
            var headers = liabilitiesAndStockholdersEquity?.units.USD.Select(u => u.end).ToList();
            var header = "Symbol\t" + headers?.Aggregate((r, n) => r + "\t" + n);
            var dataList = liabilitiesAndStockholdersEquity?.units.USD.Select(u => u.val.ToString()).ToList();
            var data = $"{symbol}" + dataList?.Aggregate((r, n) => r + "\t" + n);

            // TODO


            //return new List<string>() {
            //    "Symbol\t2024-03-31\t2024-06-30",
            //    "JNJ\t171966000000\t181088000000",
            //    "PG\t10\t18"};

            return new List<string>() {
                header,
                "JNJ\t171966000000\t181088000000",
                "PG\t10\t18"};

        }

        public async Task<IEnumerable<DateAndValue>> StockholdersEquity(string symbol)
        {
            string cik = "0000200406"; // TODO
            string url = $"https://data.sec.gov/api/xbrl/companyconcept/CIK{cik}/us-gaap/LiabilitiesAndStockholdersEquity.json";
            var response = await _httpClient.GetStringAsync(url);

            var liabilitiesAndStockholdersEquity = JsonSerializer.Deserialize<LiabilitiesAndStockholdersEquity>(response) ?? throw new Exception();
            var resultList = new List<DateAndValue>();
            foreach (var t in liabilitiesAndStockholdersEquity.units.USD)
            {
                resultList.Add(new DateAndValue(t.end, t.val.ToString() ?? ""));
            }

            return resultList;
        }
    }
}

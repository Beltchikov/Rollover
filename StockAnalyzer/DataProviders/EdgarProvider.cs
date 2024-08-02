using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    internal class EdgarProvider : IEdgarProvider
    {
        public async Task<IEnumerable<string>> StockholdersEquity(string cik)
        {
            return await Task.Run(() =>
            {
                return new List<string>() {
                "JNJ\t171966000000\t181088000000",
                "PG\t10\t18"};
            });
        }
    }
}

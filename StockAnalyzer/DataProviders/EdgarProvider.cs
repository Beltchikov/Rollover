using StockAnalyzer.DataProviders.Types;
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
                
                var liabilitiesAndStockholdersEquity = new LiabilitiesAndStockholdersEquity();



                return new List<string>() {
                "Symbol\t2024-03-31\t2024-06-30",
                "JNJ\t171966000000\t181088000000",
                "PG\t10\t18"};
            });
        }
    }
}

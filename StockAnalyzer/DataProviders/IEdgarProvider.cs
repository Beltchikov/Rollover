using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public interface IEdgarProvider
    {
        Task<IEnumerable<string>> StockholdersEquity(string cik);
    }
}

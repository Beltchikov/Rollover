using StockAnalyzer.DataProviders.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public interface IEdgarProvider
    {
        Task<IEnumerable<string>> StockholdersEquityOld(string cik);
        Task<IEnumerable<DateAndValue>> StockholdersEquity(string cik);
    }
}

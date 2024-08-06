using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public interface IEdgarProvider
    {
        Task<string> Cik(string symbol);
        IEnumerable<string> InterpolateDataForMissingDates(List<string> data);
        Task<IEnumerable<string>> StockholdersEquity(string symbol);
        Task<IEnumerable<string>> StockholdersEquity(List<string> symbolList);
    }
}

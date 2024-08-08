using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public interface IEdgarProvider
    {
        Task<string> Cik(string symbol);
        Task<IEnumerable<string>> Dividends(List<string> list);
        IEnumerable<string> InterpolateDataForMissingDates(List<string> data);
        Task<IEnumerable<string>> LongTermDebt(List<string> list);
        Task<IEnumerable<string>> NetIncome(List<string> list);
        Task<IEnumerable<string>> StockholdersEquity(string symbol);
        Task<IEnumerable<string>> BatchProcessing(List<string> symbolList, Func<string, Task<IEnumerable<string>>> processingFunc);
    }
}

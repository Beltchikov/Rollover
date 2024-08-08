using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public interface IEdgarProvider
    {
        Task<string> Cik(string symbol);
        Task<IEnumerable<string>> BatchProcessing(List<string> symbolList, Func<string, Task<IEnumerable<string>>> processingFunc);
        Task<IEnumerable<string>> StockholdersEquity(string symbol);
        Task<IEnumerable<string>> LongTermDebt(string symbol);
        Task<IEnumerable<string>> Dividends(string symbol);
        Task<IEnumerable<string>> NetIncome(string symbol);
        IEnumerable<string> InterpolateDataForMissingDates(List<string> data);

    }
}

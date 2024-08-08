using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public interface IEdgarProvider
    {
        Task<string> Cik(string symbol);
        Task<IEnumerable<string>> BatchProcessing(
           List<string> symbolList,
           string companyConcept,
           Func<string, string, Task<IEnumerable<string>>> processingFunc);
        Task<IEnumerable<string>> CompanyConcept(string symbol, string CompanyConcepc);
        IEnumerable<string> InterpolateDataForMissingDates(List<string> data);

    }
}

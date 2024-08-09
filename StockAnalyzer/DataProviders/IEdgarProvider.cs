using StockAnalyzer.DataProviders.Types;
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
           Func<string, string, Task<WithError<IEnumerable<string>>>> processingFunc);
        Task<WithError<IEnumerable<string>>> CompanyConceptOrError(string symbol, string companyConcept);
        IEnumerable<string> InterpolateDataForMissingDates(List<string> data);

    }
}

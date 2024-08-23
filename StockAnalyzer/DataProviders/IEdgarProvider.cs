using StockAnalyzer.DataProviders.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public interface IEdgarProvider
    {
        Task<string> Cik(string symbol);
        Task<IEnumerable<WithError<string?>>> BatchProcessing(
           List<string> symbolList,
           List<string> companyConceptArray,
           Func<string, string, Task<WithError<IEnumerable<string>>>> processingFunc);
        Task<WithError<IEnumerable<string>>> CompanyConceptOrError(string symbol, string companyConcept);
        IEnumerable<string> InterpolateDataForMissingDates(List<string> data);
    }
}

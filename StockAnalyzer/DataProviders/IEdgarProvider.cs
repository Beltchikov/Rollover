using StockAnalyzer.DataProviders.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static StockAnalyzer.DataProviders.EdgarProvider;

namespace StockAnalyzer.DataProviders
{
    public interface IEdgarProvider
    {
        Task<string> Cik(string symbol);
        Task<IEnumerable<WithError<string?>>> BatchProcessing(
           List<string> symbolList,
           List<string> companyConceptArray,
           ConceptFuncDelegate conceptFunc);
        Task<WithError<IEnumerable<string>>> CompanyConceptOrError(string symbol, string companyConcept);
        IEnumerable<string> InterpolateDataForMissingDates(List<string> data);
    }
}

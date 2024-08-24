using StockAnalyzer.DataProviders.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using static StockAnalyzer.DataProviders.EdgarProvider;

namespace StockAnalyzer.DataProviders
{
    public interface IEdgarProvider
    {
        Task<string> Cik(string symbol);
        BatchProcessingDelegate BatchProcessing { get;}   
        Task<WithError<IEnumerable<string>>> CompanyConceptOrError(string symbol, string companyConcept);
        IEnumerable<string> InterpolateDataForMissingDates(List<string> data);
    }
}

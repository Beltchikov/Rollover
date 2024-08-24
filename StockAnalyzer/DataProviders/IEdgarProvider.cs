using System.Collections.Generic;
using System.Threading.Tasks;
using static StockAnalyzer.DataProviders.EdgarProvider;

namespace StockAnalyzer.DataProviders
{
    public interface IEdgarProvider
    {
        Task<string> Cik(string symbol);
        BatchProcessingDelegate BatchProcessing { get;}   
        ConceptFuncDelegate CompanyConceptOrError { get; }
        IEnumerable<string> InterpolateDataForMissingDates(List<string> data);
    }
}

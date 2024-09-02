using System.Collections.Generic;
using System.Threading.Tasks;
using static StockAnalyzer.DataProviders.EdgarProvider;

namespace StockAnalyzer.DataProviders
{
    public interface IEdgarProvider
    {
        Task<string> Cik(string symbol);
        SimpleBatchProcessingDelegate SimpleBatchProcessing { get;}   
        ComputedBatchProcessingDelegate ComputedBatchProcessing { get;}   
        ConceptFuncDelegate CompanyConceptOrError { get; }
        IEnumerable<string> InterpolateDataForMissingDates(List<string> data);
        IEnumerable<string> Cagr(List<string> inputList, int periods);
    }
}

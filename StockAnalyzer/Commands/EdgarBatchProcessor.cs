using StockAnalyzer.DataProviders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static StockAnalyzer.DataProviders.EdgarProvider;

namespace StockAnalyzer.Commands
{
    public class EdgarBatchProcessor
    {
        public static async Task RunBatchProcessingAsync(
            IEdgarConsumer edgarConsumer,
            List<string> companyConceptArray,
            BatchProcessingDelegate batchProcessingFunc)
        {
           List<WithError<string?>> batchProcessingResults = (await batchProcessingFunc(
                                    edgarConsumer.TickerCollectionEdgar.ToList(),
                                    companyConceptArray))?.ToList() ?? throw new ApplicationException();
            List<string> data = batchProcessingResults
                            .Where(x => x.Data != null)
                            .Select(r => r.Data ?? "")
                            .ToList();
            List<string> errors = batchProcessingResults
                            .Where(x => x.Error != null)
                            .Select(r => r.Error ?? "")
                            .ToList();

            if(errors.Any()) edgarConsumer.AddMessageEdgar(errors.Aggregate((r,n)=> r + "\r\n"+n));
            edgarConsumer.ResultCollectionEdgar = new ObservableCollection<string>(data);
            edgarConsumer.ResultsCalculatedEdgar = true;
        }
    }
}

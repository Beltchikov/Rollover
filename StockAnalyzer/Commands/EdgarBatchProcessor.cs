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
        public static async Task RunSimpleBatchProcessingAsync(
            IEdgarConsumer edgarConsumer,
            SimpleAccountingAttribute accountingAttribute,
            SimpleBatchProcessingDelegate batchProcessingFunc)
        {
            List<WithError<string?>> batchProcessingResults = (await batchProcessingFunc(
                                     edgarConsumer.TickerCollectionEdgar.ToList(),
                                     accountingAttribute.OtherNames))?.ToList() ?? throw new ApplicationException();
            List<string> data = batchProcessingResults
                            .Where(x => x.Data != null)
                            .Select(r => r.Data ?? "")
                            .ToList();
            List<string> errors = batchProcessingResults
                            .Where(x => x.Error != null)
                            .Select(r => r.Error ?? "")
                            .ToList();

            if (errors.Any()) edgarConsumer.AddMessageEdgar(errors.Aggregate((r, n) => r + "\r\n" + n));
            edgarConsumer.ResultCollectionEdgar = new ObservableCollection<string>(data);
            edgarConsumer.ResultsCalculatedEdgar = true;
        }

        public static async Task RunComputedBatchProcessingAsync(
            IEdgarConsumer edgarConsumer,
            ComputedAccountingAttribute computedAccountingAttribute,
            ComputedBatchProcessingDelegate batchProcessingFunc)
        {
            List<WithError<string?>> batchProcessingResults = (await batchProcessingFunc(
                                     edgarConsumer.TickerCollectionEdgar.ToList(),
                                     computedAccountingAttribute.OtherNames1,
                                     computedAccountingAttribute.OtherNames2,
                                     computedAccountingAttribute.computeFunc,
                                     computedAccountingAttribute.Labels))?.ToList() ?? throw new ApplicationException();
            List<string> data = batchProcessingResults
                            .Where(x => x.Data != null)
                            .Select(r => r.Data ?? "")
                            .ToList();
            List<string> errors = batchProcessingResults
                            .Where(x => x.Error != null)
                            .Select(r => r.Error ?? "")
                            .ToList();

            if (errors.Any()) edgarConsumer.AddMessageEdgar(errors.Aggregate((r, n) => r + "\r\n" + n));
            edgarConsumer.ResultCollectionEdgar = new ObservableCollection<string>(data);
            edgarConsumer.ResultsCalculatedEdgar = true;
        }

        public record SimpleAccountingAttribute(string Name, List<string> OtherNames);
        public record ComputedAccountingAttribute(
            string Name,
            List<string> OtherNames1,
            List<string> OtherNames2,
            Func<long, long, long> computeFunc,
            ThreeLabels Labels);
    }
}

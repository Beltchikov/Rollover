using StockAnalyzer.DataProviders.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using static StockAnalyzer.DataProviders.EdgarProvider;

namespace StockAnalyzer.Commands
{
    public class EdgarBatchProcessor
    {
        public static async Task RunAsync(
            IEdgarConsumer edgarConsumer,
            List<string> companyConceptArray,
            BatchProcessingDelegate batchProcessingFunc)
        {
            bool waiting = true;
            Cursor previousCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
            edgarConsumer.BackgroundResults = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC4C5C5"));

            _ = Task.Run(() =>
            {
                while (waiting)
                {
                    edgarConsumer.ProgressBarValue += 1;
                    Thread.Sleep(200);
                };
            });
            edgarConsumer.ResultCollectionEdgar = new ObservableCollection<string>(Enumerable.Empty<string>());

            //
            List<WithError<string?>> batchProcessingResults = (await batchProcessingFunc(
                                    edgarConsumer.TickerCollectionEdgar.ToList(),
                                    companyConceptArray,
                                    edgarConsumer.EdgarProvider.CompanyConceptOrError))?.ToList() ?? throw new ApplicationException();
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

            //
            waiting = false;
            Mouse.OverrideCursor = previousCursor;
            edgarConsumer.BackgroundResults = new SolidColorBrush(Colors.White);
            edgarConsumer.ProgressBarValue = 0;
        }
    }
}

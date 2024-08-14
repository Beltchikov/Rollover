using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace StockAnalyzer.Commands
{
    public class EdgarBatchProcessor
    {
        public static async Task RunAsync(IEdgarConsumer edgarConsumer, string[] companyConceptArray)
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

            edgarConsumer.ResultCollectionEdgar = new ObservableCollection<string>(
                                await edgarConsumer.EdgarProvider.BatchProcessing(
                                    edgarConsumer.TickerCollectionEdgar.ToList(),
                                    companyConceptArray,
                                    edgarConsumer.EdgarProvider.CompanyConceptOrError));

            waiting = false;
            Mouse.OverrideCursor = previousCursor;
            edgarConsumer.BackgroundResults = new SolidColorBrush(Colors.White);
            edgarConsumer.ProgressBarValue = 0;
        }
    }
}

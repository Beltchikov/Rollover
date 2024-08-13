using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace StockAnalyzer.Commands
{
    public class Dividends
    {
        public static async Task RunAsync(IDividendsConsumer dividendsConsumer)
        {
            // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/PaymentsOfDividends.json

            bool waiting = true;
            Cursor previousCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
            Mouse.OverrideCursor = Cursors.Wait;
            dividendsConsumer.BackgroundResults = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC4C5C5"));

            _ = Task.Run(() =>
            {
                while (waiting)
                {
                    dividendsConsumer.ProgressBarValue += 1;
                    Thread.Sleep(200);
                };
            });

            dividendsConsumer.ResultCollectionEdgar = new ObservableCollection<string>(Enumerable.Empty<string>());
            dividendsConsumer.ResultCollectionEdgar = new ObservableCollection<string>(
                                await dividendsConsumer.EdgarProvider.BatchProcessing(
                                    dividendsConsumer.TickerCollectionEdgar.ToList(),
                                    new string[] { "DividendsCommonStockCash", "DividendsCash", "Dividends", "PaymentsOfDividends" },
                                    dividendsConsumer.EdgarProvider.CompanyConceptOrError));

            waiting = false;
            Mouse.OverrideCursor = previousCursor;
            dividendsConsumer.BackgroundResults = new SolidColorBrush(Colors.White);
            dividendsConsumer.ProgressBarValue = 0;
        }
    }
}

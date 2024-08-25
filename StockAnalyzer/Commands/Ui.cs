using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace StockAnalyzer.Commands
{
    public class Ui
    {
        bool waiting = true;
        Cursor previousCursor = null!;

        public void Disable(IEdgarConsumer edgarConsumer, int progressBarDelay)
        {
            previousCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
            edgarConsumer.BackgroundResults = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC4C5C5"));

            _ = Task.Run(() =>
            {
                while (waiting)
                {
                    edgarConsumer.ProgressBarValue += 1;
                    Thread.Sleep(progressBarDelay);
                };
            });

            edgarConsumer.ResultCollectionEdgar = new ObservableCollection<string>(Enumerable.Empty<string>());
        }

        public void Enable(IEdgarConsumer edgarConsumer)
        {
            waiting = false;
            Mouse.OverrideCursor = previousCursor;
            edgarConsumer.BackgroundResults = new SolidColorBrush(Colors.White);
            edgarConsumer.ProgressBarValue = 0;
        }
    }
}

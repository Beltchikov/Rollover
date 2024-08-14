using StockAnalyzer.DataProviders;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace StockAnalyzer.Commands
{
    public interface IEdgarConsumer
    {
        ObservableCollection<string> TickerCollectionEdgar { get; set; }
        ObservableCollection<string> ResultCollectionEdgar { get; set; }
        IEdgarProvider EdgarProvider { get; }
        Brush BackgroundResults { get; set; }
        int ProgressBarValue { get; set; }
    }
}

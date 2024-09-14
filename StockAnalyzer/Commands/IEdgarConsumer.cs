using StockAnalyzer.DataProviders;
using StockAnalyzer.Repositories;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace StockAnalyzer.Commands
{
    public interface IEdgarConsumer
    {
        ObservableCollection<string> TickerCollectionEdgar { get; set; }
        ObservableCollection<string> ResultCollectionEdgar { get; set; }
        bool ResultsCalculatedEdgar { get; set; }
        bool ResultsCalculatedEdgarMultipleTables { get; set; }
        IEdgarProvider EdgarProvider { get; }
        Brush BackgroundResults { get; set; }
        int ProgressBarValue { get; set; }
        IRepository Repositry { get; }
        void AddMessageEdgar(string message);
    }
}

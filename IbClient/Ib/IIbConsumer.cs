using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StockAnalyzer.Ib
{
    public interface IIbConsumer
    {
        bool ConnectedToTws { get; set; }
        ObservableCollection<string> TwsMessageCollection{ get; set; }
    }
}

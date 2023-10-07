using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StockAnalyzer.Ib
{
    public interface IIbConsumer
    {
        public bool ConnectedToTws { get; set; }
        public ObservableCollection<string> TwsMessageCollection{ get; internal set; }
    }
}

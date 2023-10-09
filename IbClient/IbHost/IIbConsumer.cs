using System.Collections.ObjectModel;

namespace IbClient.IbHost
{
    public interface IIbConsumer
    {
        bool ConnectedToTws { get; set; }
        ObservableCollection<string> TwsMessageCollection { get; set; }
    }
}

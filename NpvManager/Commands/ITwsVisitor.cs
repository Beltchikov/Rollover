using IbClient.IbHost;
using System.Collections.ObjectModel;

namespace NpvManager.Commands
{
    public interface ITwsVisitor
    {
        IIbHost IbHost { get; } 
        string Host { get; } 
        int Port { get; } 
        int ClientId { get; } 
        int Timeout { get; }
        
        bool ConnectedToTws { get; }
        ObservableCollection<string> TwsMessageCollection { get; }
    }
}

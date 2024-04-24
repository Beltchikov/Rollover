using IbClient.IbHost;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrader.Commands
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

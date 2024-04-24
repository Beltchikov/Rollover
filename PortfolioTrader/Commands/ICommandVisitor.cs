using IbClient.IbHost;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrader.Commands
{
    interface ICommandVisitor
    {
        IIbHost IbHost { get; } 
        string Host { get; } 
        int Port { get; } 
        int ClientId { get; } 
        int Timeout { get; }
        string LongSymbolsAsString { get; set; }
        string ShortSymbolsAsString { get; set; }
        ObservableCollection<string> TwsMessageCollection { get; }
        string LongSymbolsResolved { get; set; }
        string LongSymbolsMultiple { get; set; }
        string LongSymbolsUnresolved { get; set; }
        string ShortSymbolsResolved { get; set; }
        string ShortSymbolsMultiple { get; set; }
        string ShortSymbolsUnresolved { get; set; }

    }
}

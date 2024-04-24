using IbClient.IbHost;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrader.Commands
{
    public interface IBuyModelVisitor : ITwsVisitor
    {
        string LongSymbolsAsString { get; set; }
        string ShortSymbolsAsString { get; set; }
        string LongSymbolsResolved { get; set; }
        string LongSymbolsMultiple { get; set; }
        string LongSymbolsUnresolved { get; set; }
        string ShortSymbolsResolved { get; set; }
        string ShortSymbolsMultiple { get; set; }
        string ShortSymbolsUnresolved { get; set; }

    }
}

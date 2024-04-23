using IbClient.IbHost;
using System;
using System.Collections.Generic;
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

    }
}

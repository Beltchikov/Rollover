﻿using IbClient.IbHost;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioTrader.Commands
{
    interface ITwsVisitor
    {
        IIbHost IbHost { get; } 
        string Host { get; } 
        int Port { get; } 
        int ClientId { get; } 
        int Timeout { get; }

        ObservableCollection<string> TwsMessageCollection { get; }
    }
}

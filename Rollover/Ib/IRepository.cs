﻿using IBApi;
using IBSampleApp.messages;
using Rollover.Tracking;
using System;
using System.Collections.Generic;

namespace Rollover.Ib
{
    public interface IRepository
    {
        Tuple<bool, List<string>> Connect(string host, int port, int clientId);
        void Disconnect();
        List<PositionMessage> AllPositions();
        List<SecurityDefinitionOptionParameterMessage> OptionParameters(
           string symbol,
           string exchange,
           string secType,
           int conId);
        ITrackedSymbol GetTrackedSymbol(Contract contract);
    }
}
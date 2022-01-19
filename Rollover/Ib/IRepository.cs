using IBApi;
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
        TrackedSymbol GetTrackedSymbol(Contract contract);

        Tuple<bool, double> GetCurrentPrice(int conId, string exchange);
        void PlaceBearSpread(Contract contract, int v1, int v2);
    }
}
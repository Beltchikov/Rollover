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
        List<ContractDetailsMessage> ContractDetails(Contract contract);
        List<SecurityDefinitionOptionParameterMessage> OptionParameters(
           string symbol,
           string exchange,
           string secType,
           int conId);
        TrackedSymbol GetTrackedSymbol(Contract contract);
        HashSet<double> GetStrikes(Contract contract, string lastTradeDateOrContractMonth);

        Tuple<bool, double> GetCurrentPrice(int conId, string exchange);
        void PlaceBearSpread(ITrackedSymbol trackedSymbol);
    }
}
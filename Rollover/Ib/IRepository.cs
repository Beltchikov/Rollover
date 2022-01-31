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
        HashSet<double> GetStrikes(Contract contract, string lastTradeDateOrContractMonth);

        Tuple<bool, double> LastPrice(int conId, string exchange);
        Tuple<bool, double> ClosePrice(int conId, string exchange);
        Tuple<bool, double> BidPrice(int conId, string exchange);
        Tuple<bool, double> AskPrice(int conId, string exchange);
        Tuple<bool, double> BidPrice(Contract contract);
        Tuple<bool, double> AskPrice(Contract contract);
        void PlaceBearSpread(ITrackedSymbol trackedSymbol);
        bool IsConnected();
    }
}
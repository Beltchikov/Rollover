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

        Result<double> LastPrice(int conId, string exchange);
        Result<double> ClosePrice(int conId, string exchange);
        Result<double> BidPrice(int conId, string exchange);
        Result<double> AskPrice(int conId, string exchange);
        Result<double> BidPrice(Contract contract);
        Result<double> AskPrice(Contract contract);
        void PlaceBearSpread(ITrackedSymbol trackedSymbol);
        bool IsConnected();
        void PlaceOrder(Contract contractCall, Order orderCall);
        bool MarketIsOpen(ContractDetailsMessage contractDetailsMessage, DateTime dateTime);
    }
}
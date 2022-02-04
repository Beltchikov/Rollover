using IBApi;
using IBSampleApp.messages;
using System;
using System.Collections.Generic;

namespace Rollover.Ib
{
    public interface IMessageCollector
    {
        ConnectionMessages eConnect(string host, int port, int clientId);
        List<PositionMessage> reqPositions();
        List<ContractDetailsMessage> reqContractDetails(Contract contract);
        List<SecurityDefinitionOptionParameterMessage> reqSecDefOptParams(
            string symbol, 
            string exchange, 
            string secType, 
            int conId);
        Tuple<List<TickSizeMessage>, List<TickPriceMessage>> reqMktData(
            Contract contract, 
            string generickTickList, 
            bool snapshot, 
            bool regulatorySnapshot,
            List<TagValue> mktDataOptions);
        bool IsConnected();
        void placeOrder(Contract contract, Order order);
        void reqId();
    }
}
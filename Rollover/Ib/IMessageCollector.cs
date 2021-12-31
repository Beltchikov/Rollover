using IBApi;
using IBSampleApp.messages;
using System.Collections.Generic;

namespace Rollover.Ib
{
    public interface IMessageCollector
    {
        ConnectionMessages eConnect(string host, int port, int clientId);
        List<PositionMessage> reqPositions();
        List<ContractDetailsMessage> reqContractDetails(int reqId, Contract contract);
    }
}
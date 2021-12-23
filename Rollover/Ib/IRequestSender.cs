using IBApi;
using System.Threading;

namespace Rollover.Ib
{
    public interface IRequestSender
    {
        void RegisterResponseHandlers(Input.IInputQueue _inputQueue, SynchronizationContext synchronizationContext);
        void Connect(string host, int port, int clientId);
        void Disconnect();
        void ListPositions();
        void ReqSecDefOptParams(int reqId, string symbol, string exchange, string secType, int conId);
        void ContractDetails(int reqId, Contract contract); 
    }
}
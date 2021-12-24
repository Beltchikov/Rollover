using IBApi;
using Rollover.Input;
using System.Threading;

namespace Rollover.Ib
{
    public interface IRepository
    {
        bool Connect(string host, int port, int clientId, IInputQueue inputQueue, int timeout);
        void Disconnect();
        void ListPositions();
        void ReqSecDefOptParams(int reqId, string symbol, string exchange, string secType, int conId);
        void ContractDetails(int reqId, Contract contract); 
    }
}
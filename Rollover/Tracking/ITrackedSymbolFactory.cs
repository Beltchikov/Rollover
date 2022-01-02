using IBApi;
using IBSampleApp.messages;

namespace Rollover.Tracking
{
    public interface ITrackedSymbolFactory
    {
        TrackedSymbol Create(string symbol);
        TrackedSymbol InitFromContractDetailsMessage(ContractDetailsMessage contractDetailsMessage);
        TrackedSymbol InitFromContract(Contract contract);
    }
}
using IBSampleApp.messages;

namespace Rollover.Tracking
{
    public interface ITrackedSymbolFactory
    {
        TrackedSymbol Create(string symbol);
        TrackedSymbol FromContractDetailsMessage(ContractDetailsMessage contractDetailsMessage);
    }
}
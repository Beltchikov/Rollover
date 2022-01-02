using IBApi;
using IBSampleApp.messages;
using System.Collections.Generic;

namespace Rollover.Tracking
{
    public interface ITrackedSymbolFactory
    {
        TrackedSymbol Create(string symbol);
        TrackedSymbol InitFromContractDetailsMessage(ContractDetailsMessage contractDetailsMessage);
        TrackedSymbol InitFromContract(Contract contract);
        TrackedSymbol Create(Contract contract, HashSet<double> strikes, double currentPrice);
    }
}
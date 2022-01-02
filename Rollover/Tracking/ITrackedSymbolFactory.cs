using IBApi;
using IBSampleApp.messages;
using System.Collections.Generic;

namespace Rollover.Tracking
{
    public interface ITrackedSymbolFactory
    {
        TrackedSymbol Create(Contract contract, HashSet<double> strikes, double currentPrice);
    }
}
using IBApi;
using IBSampleApp.messages;
using System.Collections.Generic;

namespace Rollover.Tracking
{
    public interface ITrackedSymbolFactory
    {
        TrackedSymbol Create(Contract underlyingContract, HashSet<double> strikes, double currentPrice);
        double PreviousStrike(HashSet<double> strikes, double currentPrice);
    }
}
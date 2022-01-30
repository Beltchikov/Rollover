using Rollover.Ib;
using System.Collections.Generic;

namespace Rollover.Tracking
{
    public interface ITrackedSymbol
    {
        string LocalSymbol { get; }
        int ConId { get; }
        string Exchange { get; }

        string Currency(IRepository repository);
        string LastTradeDateOrContractMonth(IRepository repository);
        double NextButOneStrike(IRepository repository, double currentPrice);
        double NextStrike(IRepository repository, double currentPrice);
        double PreviousButOneStrike(IRepository repository, double currentPrice);
        double PreviousStrike(IRepository repository, double currentPrice);
        void ResetCache();
        string Right(IRepository repository);
        string SecType(IRepository repository);
        double? Strike(IRepository repository);
        HashSet<double> Strikes(IRepository repository);
        string Symbol(IRepository repository);
        string ToString();
        bool Equals(ITrackedSymbol otherSymbol);
        double LastUnderlyingPrice(IRepository repository);
    }
}
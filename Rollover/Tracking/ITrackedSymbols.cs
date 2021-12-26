using System.Collections.Generic;

namespace Rollover.Tracking
{
    public interface ITrackedSymbols
    {
        bool SymbolExists(string input);
        IEnumerable<string> AllAsString();
        bool Add(ITrackedSymbol trackedSymbol);
    }
}
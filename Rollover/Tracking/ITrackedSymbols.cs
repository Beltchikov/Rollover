using System.Collections.Generic;

namespace Rollover.Tracking
{
    public interface ITrackedSymbols
    {
        bool SymbolExists(string input);
        bool Add(ITrackedSymbol trackedSymbol);
        IEnumerable<string> List();
        List<string> Summary();
        bool Any();
    }
}
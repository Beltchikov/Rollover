using System.Collections.Generic;

namespace Rollover.Tracking
{
    public interface ITrackedSymbols : IEnumerable<TrackedSymbol>
    {
        bool SymbolExists(string input);
        bool Add(TrackedSymbol trackedSymbol);
        IEnumerable<string> List();
        List<string> Summary();
        bool Any();
    }
}
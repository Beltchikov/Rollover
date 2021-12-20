using System.Collections.Generic;

namespace Rollover.Tracking
{
    public interface ITrackedSymbols
    {
        bool SymbolExists(string input);
        void Add(string input);
        IEnumerable<string> AllAsString();
    }
}
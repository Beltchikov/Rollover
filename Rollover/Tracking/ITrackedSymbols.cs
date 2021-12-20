using System.Collections.Generic;

namespace Rollover.Tracking
{
    public interface ITrackedSymbols
    {
        bool SymbolExists(string input);
        void BeginAdd(string input);
        IEnumerable<string> AllAsString();
    }
}
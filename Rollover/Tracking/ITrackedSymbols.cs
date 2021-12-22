using System.Collections.Generic;

namespace Rollover.Tracking
{
    public interface ITrackedSymbols
    {
        bool SymbolExists(string input);
        void BeginAdd(int reqId, string symbol, string exchange, string secType, int conId);
        IEnumerable<string> AllAsString();
    }
}
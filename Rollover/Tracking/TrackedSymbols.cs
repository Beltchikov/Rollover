using Rollover.Ib;
using System.Collections.Generic;
using System.Linq;

namespace Rollover.Tracking
{
    public class TrackedSymbols : ITrackedSymbols
    {
        private readonly List<TrackedSymbol> _symbols;

        public TrackedSymbols()
        {
            _symbols = new List<TrackedSymbol>();
        }

        public IEnumerable<string> AllAsString()
        {
            var allAsString = _symbols.Select(s => s.ToString()).ToList();
            allAsString.Sort();
            return allAsString;
        }

        public bool SymbolExists(string symbol)
        {
            return _symbols.Any(s => s.Symbol == symbol);
        }
    }
}

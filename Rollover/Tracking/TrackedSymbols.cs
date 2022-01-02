using System.Collections.Generic;
using System.Linq;

namespace Rollover.Tracking
{
    public class TrackedSymbols : ITrackedSymbols
    {
        private readonly HashSet<ITrackedSymbol> _symbols;

        public TrackedSymbols()
        {
            _symbols = new HashSet<ITrackedSymbol>();
        }

        public bool Add(ITrackedSymbol trackedSymbol)
        {
            if (_symbols.Any(s => s.LocalSymbol == trackedSymbol.LocalSymbol))
            {
                return false;
            }

            return _symbols.Add(trackedSymbol);
        }

        public IEnumerable<string> List()
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

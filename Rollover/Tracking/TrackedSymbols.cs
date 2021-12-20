using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public void Add(string input)
        {
            // TODO Factory
            var newSymbol = new TrackedSymbol { Name = input };
            _symbols.Add(newSymbol);
        }

        public IEnumerable<string> AllAsString()
        {
            var allAsString = _symbols.Select(s => s.ToString()).ToList();
            allAsString.Sort();
            return allAsString;
        }

        public bool SymbolExists(string input)
        {
            return _symbols.Any(s => s.Name == input);
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rollover.Tracking
{
    public class TrackedSymbols : ITrackedSymbols
    {
        private readonly ITrackedSymbolFactory _factory;
        private readonly List<TrackedSymbol> _symbols;

        public TrackedSymbols(ITrackedSymbolFactory factory)
        {
            _factory = factory;
            _symbols = new List<TrackedSymbol>();
        }

        public void Add(string input)
        {
            var newSymbol = _factory.Create(input);
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

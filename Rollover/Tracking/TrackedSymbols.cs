using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rollover.Tracking
{
    [ExcludeFromCodeCoverage(Justification ="It's a wrapper")]
    public class TrackedSymbols : ITrackedSymbols
    {
        private readonly List<string> _symbols;

        public TrackedSymbols()
        {
            _symbols = new List<string>();
        }
        
        public void Add(string input)
        {
            _symbols.Add(input);
        }

        public bool SymbolExists(string input)
        {
            return _symbols.Contains(input);
        }
    }
}

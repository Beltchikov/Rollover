using Rollover.Ib;
using System.Collections.Generic;
using System.Linq;

namespace Rollover.Tracking
{
    public class TrackedSymbols : ITrackedSymbols
    {
        private readonly IRequestSender _requestSender;
        private readonly List<TrackedSymbol> _symbols;

        public TrackedSymbols(IRequestSender requestSender)
        {
            _requestSender = requestSender;
            _symbols = new List<TrackedSymbol>();
        }

        public void BeginAdd(string input)
        {
            // TODO
            //_requestSender.ReqSecDefOptParams.Add(input);
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

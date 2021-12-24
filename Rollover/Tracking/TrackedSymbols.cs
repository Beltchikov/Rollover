using Rollover.Ib;
using System.Collections.Generic;
using System.Linq;

namespace Rollover.Tracking
{
    public class TrackedSymbols : ITrackedSymbols
    {
        private readonly IRepository _requestSender;
        private readonly List<TrackedSymbol> _symbols;

        public TrackedSymbols(IRepository requestSender)
        {
            _requestSender = requestSender;
            _symbols = new List<TrackedSymbol>();
        }

        public void BeginAdd(int reqId, string symbol, string exchange, string secType, int conId)
        {
            _requestSender.ReqSecDefOptParams(reqId, symbol, exchange, secType, conId);
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

using Rollover.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Rollover.Tracking
{
    public class TrackedSymbols : ITrackedSymbols
    {
        private readonly HashSet<ITrackedSymbol> _symbols;
        private IFileHelper _fileHelper;
        private ISerializer _serializer;
        private string TRACKED_SYMBOLS_FILE = "TrackedSymbols.json";

        public TrackedSymbols(IFileHelper fileHelper, ISerializer serializer)
        {
            _fileHelper = fileHelper;
            _serializer = serializer;

            if (_fileHelper.FileExists(TRACKED_SYMBOLS_FILE))
            {
                var trackedSymbolsAsText = _fileHelper.ReadAllText(TRACKED_SYMBOLS_FILE);
                _symbols = _serializer.Deserialize<HashSet<ITrackedSymbol>>(trackedSymbolsAsText);
            }
            else
            {
                _symbols = new HashSet<ITrackedSymbol>();
            }
        }

        public bool Add(ITrackedSymbol trackedSymbol)
        {
            if (_symbols.Any(s => s.LocalSymbol == trackedSymbol.LocalSymbol))
            {
                return false;
            }
            
            bool addResult = _symbols.Add(trackedSymbol);
            string trackedSymbolsAsText = _serializer.Serialize(_symbols);
            _fileHelper.WriteAllText(TRACKED_SYMBOLS_FILE, trackedSymbolsAsText);

            return addResult;
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

using Rollover.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rollover.Tracking
{
    public class TrackedSymbols : ITrackedSymbols
    {
        private readonly HashSet<TrackedSymbol> _symbols;
        private IFileHelper _fileHelper;
        private ISerializer _serializer;
        private string TRACKED_SYMBOLS_FILE = "TrackedSymbols.json";

        public TrackedSymbols(IFileHelper fileHelper, ISerializer serializer)
        {
            _fileHelper = fileHelper;
            _serializer = serializer;

            _symbols = new HashSet<TrackedSymbol>();

            if (_fileHelper.FileExists(TRACKED_SYMBOLS_FILE))
            {
                var trackedSymbolsAsText = _fileHelper.ReadAllText(TRACKED_SYMBOLS_FILE);
                var symbols = _serializer.Deserialize<HashSet<TrackedSymbol>>(trackedSymbolsAsText).ToList();
                if (symbols.Any())
                {
                    symbols.ForEach(s => _symbols.Add(s));
                }
            }
        }

        public bool Add(TrackedSymbol trackedSymbol)
        {
            if (_symbols.Any(s => s.Equals(trackedSymbol)))
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
            return _symbols.Any(s => s.Equals(symbol));
        }

        public List<string> Summary()
        {
            if (!_symbols.Any())
            {
                return new List<string>();
            };

            var output = new List<string> { string.Empty, Constants.TRACKED_SYMBOLS };
            output.AddRange(_symbols.Select(s => s.ToString() ));
            return output;
        }

        public bool Any()
        {
            return _symbols.Any();
        }

        public IEnumerator<TrackedSymbol> GetEnumerator()
        {
            return _symbols.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Remove(string localSymbol)
        {
            var itemToRemove = _symbols.FirstOrDefault(l => l.LocalSymbol == localSymbol);
            _symbols.Remove(itemToRemove);
        }
    }
}

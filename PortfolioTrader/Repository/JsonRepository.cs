using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PortfolioTrader.Repository
{
    internal class JsonRepository : IRepository
    {
        private readonly List<KnownSymbol> _knownSymbols;
        private readonly string KNOWN_SYMBOLS_PATH = "Repository\\KnownSymbols.json";

        private readonly List<string> _notTradeableSymbols;
        private readonly string NOT_TRADEABLE_SYMBOLS_PATH = "Repository\\NotTradeableSymbols.json";

        public JsonRepository()
        {
            string dir = Directory.GetCurrentDirectory();
            
            string knownSymbolsAsString = File.ReadAllText(Path.Combine(dir, KNOWN_SYMBOLS_PATH));
            _knownSymbols = JsonSerializer.Deserialize<List<KnownSymbol>>(knownSymbolsAsString)!;

            string notTradeableSymbolsAsString = File.ReadAllText(Path.Combine(dir, NOT_TRADEABLE_SYMBOLS_PATH));
            _notTradeableSymbols = JsonSerializer.Deserialize<List<string>>(notTradeableSymbolsAsString)!;
        }

        public int? GetContractId(string symbol)
        {
            return _knownSymbols.FirstOrDefault(s=>s.Name == symbol)?.ContractId;
        }
    }
}

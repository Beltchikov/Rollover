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
        private readonly string JSON_REPOSITORY = "Repository\\KnownSymbols.json";

        public JsonRepository()
        {
            string dir = Directory.GetCurrentDirectory();
            string jsonFilePath = Path.Combine(dir, JSON_REPOSITORY);
            string knownSymbolsAsString = File.ReadAllText(jsonFilePath);
            _knownSymbols = JsonSerializer.Deserialize<List<KnownSymbol>>(knownSymbolsAsString)!;
            //ChatGptBatchResult chatGptBatchResult = JsonSerializer.Deserialize<ChatGptBatchResult>(batch)!;
        }

        public int? GetContractId(string symbol)
        {
            return _knownSymbols.FirstOrDefault(s=>s.Name == symbol)?.ContractId;
        }
    }
}

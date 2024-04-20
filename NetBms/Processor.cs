
using System.Collections.Generic;

namespace NetBms
{
    internal class Processor
    {
        public Processor()
        {
        }

        internal (SortedDictionary<string, int> buyDictionary, SortedDictionary<string, int> sellDictionary) Process(List<ChatGptBatchResult> batches)
        {
            
            SortedDictionary<string, int> sellSortedDictionary = new SortedDictionary<string, int>();

            var buySymbolsRedundant = new List<string>();
            var sellSymbolsRedundant = new List<string>();

            foreach (var batch in batches)
            {
                buySymbolsRedundant = buySymbolsRedundant.Concat(batch.BUY).ToList();
                sellSymbolsRedundant = sellSymbolsRedundant.Concat(batch.SELL).ToList();

            }

            //
            SortedDictionary<string, int> buySortedDictionary = new SortedDictionary<string, int>();
            Dictionary<string, int> resultBuyDictionary = new Dictionary<string, int>();
            foreach (var symbol in buySymbolsRedundant)
            {
                if (!buySortedDictionary.ContainsKey(symbol))
                    buySortedDictionary[symbol] = 1;
                else buySortedDictionary[symbol]++;
            }

            var buyValuesSorted = buySortedDictionary.Values.OrderDescending().ToList();

            foreach (var buyValue in buyValuesSorted)
            {
                var keyValuePair = buySortedDictionary.FirstOrDefault(d => d.Value == buyValue);
                resultBuyDictionary.Add(keyValuePair.Key, keyValuePair.Value);
                buySortedDictionary.Remove(keyValuePair.Key);
            }

            //Dictionary<string, int> resultBuyDictionary = 

            return (buySortedDictionary, sellSortedDictionary);
        }
    }
}
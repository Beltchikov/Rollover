
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
            SortedDictionary<string, int> buySortedDictionary = new SortedDictionary<string, int>();
            SortedDictionary<string, int> sellSortedDictionary = new SortedDictionary<string, int>();

            var buySymbols = new List<string>();
            var sellSymbols = new List<string>();

            foreach (var batch in batches)
            {
                buySymbols = buySymbols.Concat(batch.BUY).ToList();
                sellSymbols =sellSymbols.Concat(batch.SELL).ToList();

            }

            foreach (var symbol in buySymbols)
            {
                if(!buySortedDictionary.ContainsKey(symbol))
                    buySortedDictionary[symbol] = 1;  
                else buySortedDictionary[symbol]++;
            }

            var buyValues = buySortedDictionary.Values;
            var buyValuesSorted = buyValues.OrderDescending().ToList();

            Dictionary<string, int> buyDictionary = new Dictionary<string, int>();
            foreach (var buyValue in buyValuesSorted)
            {
                var keyValuePair = buySortedDictionary.FirstOrDefault(d => d.Value == buyValue);
                buyDictionary.Add(keyValuePair.Key, keyValuePair.Value);
                buySortedDictionary.Remove(keyValuePair.Key);

            }

            return (buySortedDictionary, sellSortedDictionary);
        }
    }
}
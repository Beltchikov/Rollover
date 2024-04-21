
using System.Collections.Generic;

namespace NetBms
{
    internal class Processor
    {
        internal (Dictionary<string, int> buyDictionary, Dictionary<string, int> sellDictionary)
            SumBuySellFromBatches(List<ChatGptBatchResult> batches)
        {
            var buySymbolsRedundant = new List<string>();
            var sellSymbolsRedundant = new List<string>();

            foreach (var batch in batches)
            {
                buySymbolsRedundant = buySymbolsRedundant.Concat(batch.BUY).ToList();
                sellSymbolsRedundant = sellSymbolsRedundant.Concat(batch.SELL).ToList();
            }

            var resultBuyDictionary = NetSignalDictionaryFromRedundantSymbolList(buySymbolsRedundant);
            var resultSellDictionary = NetSignalDictionaryFromRedundantSymbolList(sellSymbolsRedundant);

            return (resultBuyDictionary, resultSellDictionary);
        }

        internal (Dictionary<string, int> netBuySignals, Dictionary<string, int> netSellSignals)
            NetBuySellFromSum(Dictionary<string, int> sumBuySignals, Dictionary<string, int> sumSellSignals)
        {
            var netBuySignals = NetSignals(sumBuySignals, sumSellSignals);
            var netSellSignals = NetSignals(sumSellSignals, sumBuySignals);
            return (netBuySignals, netSellSignals);
        }

        private Dictionary<string, int> NetSignals(Dictionary<string, int> mainSignals, Dictionary<string, int> oppositeSignals)
        {
            Dictionary<string, int> netSignals = new();

            foreach (var signal in mainSignals)
            {
                var symbol = signal.Key;
                if (oppositeSignals.ContainsKey(symbol))
                {
                    int oppositeScore = oppositeSignals[symbol];
                    netSignals.Add(symbol, signal.Value - oppositeScore);
                }
                else
                {
                    netSignals.Add(symbol, signal.Value);
                }
            }
            return netSignals;

        }

        internal Dictionary<TKey, TValue> OrderDictionaryByValue<TKey, TValue>(
            IDictionary<TKey, TValue> unorderedDictionary,
            bool asc = true) where TKey : notnull
        {
            Dictionary<TKey, TValue> unorderdCopy = new(unorderedDictionary);

            var valuesSorted = asc
                ? unorderedDictionary.Values.Order().ToList()
                : unorderedDictionary.Values.OrderDescending().ToList();

            var resultDictionary = new Dictionary<TKey, TValue>();
            foreach (var value in valuesSorted)
            {
                KeyValuePair<TKey, TValue>? keyValuePair = unorderedDictionary
                    .FirstOrDefault(d => d.Value == null ? false : d.Value.Equals(value));
                if (keyValuePair == null) continue;

                resultDictionary.Add(keyValuePair.Value.Key, keyValuePair.Value.Value);
                unorderedDictionary.Remove(keyValuePair.Value.Key);
            }

            return resultDictionary;
        }

        private Dictionary<string, int> NetSignalDictionaryFromRedundantSymbolList(List<string> redundantSymbolList)
        {
            var sortedDictionary = SortedDictionaryFromFromRedundantSymbolList(redundantSymbolList);
            var resultDictionary = OrderDictionaryByValue(sortedDictionary, false);
            return resultDictionary;
        }

        private SortedDictionary<string, int> SortedDictionaryFromFromRedundantSymbolList(List<string> redundantSymbolList)
        {
            var sortedDictionary = new SortedDictionary<string, int>();

            foreach (var symbol in redundantSymbolList)
            {
                if (!sortedDictionary.ContainsKey(symbol))
                    sortedDictionary[symbol] = 1;
                else sortedDictionary[symbol]++;
            }

            return sortedDictionary;
        }
    }
}

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
            Dictionary<string, int> netBuySignals = new Dictionary<string, int>();
            Dictionary<string, int> netSellSignals = new Dictionary<string, int>();


            foreach (var signal in sumBuySignals)
            {
                var symbol = signal.Key;
                if (sumSellSignals.ContainsKey(symbol))
                {
                    int oppositeScore = sumSellSignals[symbol];
                    netBuySignals.Add(symbol, signal.Value - oppositeScore);
                }
                else
                {
                    netBuySignals.Add(symbol, signal.Value);
                }
            }

            return (netBuySignals, netSellSignals);
        }

        private Dictionary<string, int> NetSignalDictionaryFromRedundantSymbolList(List<string> redundantSymbolList)
        {
            var sortedDictionary = new SortedDictionary<string, int>();
            var resultDictionary = new Dictionary<string, int>();
            foreach (var symbol in redundantSymbolList)
            {
                if (!sortedDictionary.ContainsKey(symbol))
                    sortedDictionary[symbol] = 1;
                else sortedDictionary[symbol]++;
            }

            var valuesSorted = sortedDictionary.Values.OrderDescending().ToList();

            foreach (var value in valuesSorted)
            {
                var keyValuePair = sortedDictionary.FirstOrDefault(d => d.Value == value);
                resultDictionary.Add(keyValuePair.Key, keyValuePair.Value);
                sortedDictionary.Remove(keyValuePair.Key);
            }

            return resultDictionary;

        }
    }
}
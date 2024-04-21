
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
            Dictionary<string, int> netSignals = new Dictionary<string, int>();

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

        private Dictionary<string, int> NetSignalDictionaryFromRedundantSymbolList(List<string> redundantSymbolList)
        {
            //var sortedDictionary = new SortedDictionary<string, int>();
            //var resultDictionary = new Dictionary<string, int>();
            //foreach (var symbol in redundantSymbolList)
            //{
            //    if (!sortedDictionary.ContainsKey(symbol))
            //        sortedDictionary[symbol] = 1;
            //    else sortedDictionary[symbol]++;
            //}

            var sortedDictionary = SortedDictionaryFromFromRedundantSymbolList(redundantSymbolList);


            var valuesSorted = sortedDictionary.Values.OrderDescending().ToList();
            var resultDictionary = new Dictionary<string, int>();
            foreach (var value in valuesSorted)
            {
                var keyValuePair = sortedDictionary.FirstOrDefault(d => d.Value == value);
                resultDictionary.Add(keyValuePair.Key, keyValuePair.Value);
                sortedDictionary.Remove(keyValuePair.Key);
            }

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
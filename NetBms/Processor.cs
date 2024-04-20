
namespace NetBms
{
    internal class Processor
    {
        public Processor()
        {
        }

        internal (SortedDictionary<string, int> buyDictionary, SortedDictionary<string, int> sellDictionary) Process(List<ChatGptBatchResult> batches)
        {
            SortedDictionary<string, int> buyDictionary = new SortedDictionary<string, int>();
            SortedDictionary<string, int> sellDictionary = new SortedDictionary<string, int>();

            var buySymbols = new List<string>();
            var sellSymbols = new List<string>();

            foreach (var batch in batches)
            {
                buySymbols = buySymbols.Concat(batch.BUY).ToList();
                sellSymbols =sellSymbols.Concat(batch.SELL).ToList();

            }

            foreach (var symbol in buySymbols)
            {
                if(!buyDictionary.ContainsKey(symbol))
                    buyDictionary[symbol] = 1;  
                else buyDictionary[symbol]++;
            }

            //SortedDictionary<int, string> buyDictionaryReversed = new SortedDictionary<int, string>()
            //SortedDictionary<string, int> sellDictionary = new SortedDictionary<string, int>();

            return (buyDictionary, sellDictionary);
        }
    }
}
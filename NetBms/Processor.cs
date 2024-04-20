
namespace NetBms
{
    internal class Processor
    {
        public Processor()
        {
        }

        internal (Dictionary<string, int> buyDictionary, Dictionary<string, int> sellDictionary) Process(List<ChatGptBatchResult> batches)
        {
            Dictionary<string, int> buyDictionary = new Dictionary<string, int>();
            Dictionary<string, int> sellDictionary = new Dictionary<string, int>();

            var buySymbols = new List<string>();
            var sellSymbols = new List<string>();

            foreach (var batch in batches)
            {
                buySymbols = buySymbols.Concat(batch.BUY).ToList();
                sellSymbols =sellSymbols.Concat(batch.SELL).ToList();

            }

            return (buyDictionary, sellDictionary);
        }
    }
}
using IBApi;
using PortfolioTrader.Model;
using System.Windows.Controls;
using TickType = IbClient.Types.TickType;

namespace PortfolioTrader.Commands
{
    internal class CalculatePositions
    {
        private static IBuyConfirmationModelVisitor _visitor = null!;

        public static async Task RunAsync(IBuyConfirmationModelVisitor visitor)
        {
            _visitor = visitor;
            _visitor.StocksToBuyAsString = await AddPriceColumnsAsync(_visitor.StocksToBuyAsString);
            _visitor.StocksToSellAsString = await AddPriceColumnsAsync(_visitor.StocksToSellAsString);
        }

        private static async Task<string> AddPriceColumnsAsync(string stocksAsString)
        {
            var stocksToBuyDictionary = SymbolsAndScore.StringToPositionDictionary(stocksAsString);
            var resultDictionary = new Dictionary<string, Position>();
            var priceZeroList = new List<string>();
            foreach (var kvp in stocksToBuyDictionary)
            {
                if (kvp.Value.ConId == null) throw new Exception("Unexpected. Contract ID is null");
                var contract = new Contract() { ConId = kvp.Value.ConId.Value, Exchange = App.EXCHANGE };

                (double? price, TickType? tickType) = await _visitor.IbHost.RequestMktData(contract, "", true, false, null, App.TIMEOUT);
                resultDictionary[kvp.Key] = kvp.Value;

                if(price == null || price <0)
                {
                    priceZeroList.Add(kvp.Key);
                    continue;
                }

                var priceNotNullable = price == null ? 0 : price.Value;
                int priceInCents = (int)(priceNotNullable * 100d);

                int weightNotNullable = kvp.Value.Weight ?? throw new Exception("Unexpected. Weight is null");

                int quantity = 0;
                if (priceNotNullable > 0 && kvp.Value.Weight.HasValue)
                {
                    quantity = CalculateQuantity(_visitor.InvestmentAmount, priceInCents, weightNotNullable);
                }
                resultDictionary[kvp.Key].PriceInCents = priceInCents;
                resultDictionary[kvp.Key].PriceType = tickType.ToString();
                resultDictionary[kvp.Key].Quantity = quantity;
            }

            if(priceZeroList.Any()) _visitor.TwsMessageCollection.Add(
                $"Can not retrieve price for the following symbols: {priceZeroList.Aggregate((r,n)=> r+", "+n)}");

            return SymbolsAndScore.PositionDictionaryToString(resultDictionary);
        }

        private static int CalculateQuantity(int investmentAmount, int priceInCents, int weight)
        {
            var priceInDollars = (double)priceInCents / 100d;
            return (int)Math.Floor((double)(investmentAmount * weight / 100) / priceInDollars);
        }
    }
}

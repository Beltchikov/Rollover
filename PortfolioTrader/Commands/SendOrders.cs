using IBApi;
using PortfolioTrader.Model;
using System.Windows.Controls;
using TickType = IbClient.Types.TickType;

namespace PortfolioTrader.Commands
{
    internal class SendOrders
    {
        private static IBuyConfirmationModelVisitor _visitor = null!;

        public static async Task RunAsync(IBuyConfirmationModelVisitor visitor)
        {
            _visitor = visitor;
            List<TradePair> tradePairs = BuildTradePairs();

            // buy
            foreach (TradePair tradePair in tradePairs)
            {
                // Buy
                Contract contractBuy = new() { ConId = tradePair.ConIdBuy, Exchange = App.EXCHANGE, Symbol= tradePair.SymbolBuy};
                var lmtPriceBuy = Math.Round((double)tradePair.PriceInCentsBuy / 100d, 2);
                await SendOrderAndProcessResultsAsync(isLong: true, contract: contractBuy, lmtPrice: lmtPriceBuy, quantity: tradePair.QuantityBuy, visitor);

                // sell
                Contract contractSell = new() { ConId = tradePair.ConIdSell, Exchange = App.EXCHANGE, Symbol = tradePair.SymbolSell };
                var lmtPriceSell = Math.Round((double)tradePair.PriceInCentsSell / 100d, 2);
                await SendOrderAndProcessResultsAsync(isLong: false, contract: contractSell, lmtPrice: lmtPriceSell, quantity: tradePair.QuantitySell, visitor);
            }

           _visitor.TwsMessageCollection.Add($"DONE! Send Orders command executed.");
        }

        private static async Task SendOrderAndProcessResultsAsync(bool isLong, Contract contract, double lmtPrice, int quantity, IBuyConfirmationModelVisitor visitor)
        {
            Order order = new()
            {
                Action = isLong? "BUY" : "SELL",
                OrderType = "LIMIT",
                LmtPrice = lmtPrice,
                TotalQuantity = quantity
            };

            var result = await _visitor.IbHost.PlaceOrderAsync(contract, order, App.TIMEOUT);
            if (result.ErrorMessage != "")
            {
                var log = $"Error sending order for {contract.Symbol} vonId={contract.ConId} error:{result.ErrorMessage}";
                visitor.TwsMessageCollection.Add(log);
                if(isLong) visitor.OrdersLongWithError = SymbolsAndScore.ConcatStringsWithNewLine(
                    visitor.OrdersLongWithError,
                    contract.Symbol + " " + result.ErrorMessage);
                else visitor.OrdersShortWithError = SymbolsAndScore.ConcatStringsWithNewLine(
                    visitor.OrdersShortWithError,
                    contract.Symbol + " " + result.ErrorMessage);
            }
            else if (result.OrderState != null)
            {
                visitor.TwsMessageCollection.Add($"{contract.Symbol} conId={contract.ConId} order submitted.");
            }
            else throw new Exception("Unexpected. Both ErrorMessage nad OrderState are not set.");

            await Task.Run(() => Thread.Sleep(App.TIMEOUT));
        }

        private static List<TradePair> BuildTradePairs()
        {
            var buyDictionary = SymbolsAndScore.StringToPositionDictionary(_visitor.StocksToBuyAsString);
            var sellDictionary = SymbolsAndScore.StringToPositionDictionary(_visitor.StocksToSellAsString);

            List<TradePair> tradePairs = buyDictionary
                .Zip(sellDictionary, (b, s) =>
                    new TradePair(
                        b.Key,
                        s.Key,
                        b.Value.ConId.HasValue ? b.Value.ConId.Value : throw new Exception("Unexpected. Value is null"),
                        s.Value.ConId.HasValue ? s.Value.ConId.Value : throw new Exception("Unexpected. Value is null"),
                        b.Value.PriceInCents.HasValue ? b.Value.PriceInCents.Value : throw new Exception("Unexpected. Value is null"),
                        s.Value.PriceInCents.HasValue ? s.Value.PriceInCents.Value : throw new Exception("Unexpected. Value is null"),
                        b.Value.Quantity.HasValue ? b.Value.Quantity.Value : throw new Exception("Unexpected. Value is null"),
                        s.Value.Quantity.HasValue ? s.Value.Quantity.Value : throw new Exception("Unexpected. Value is null")))
                .ToList();

            return tradePairs;

        }
    }
}

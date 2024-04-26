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

            foreach (TradePair tradePair in tradePairs)
            {
                var nextOrderId = await visitor.IbHost.ReqIdsAsync(-1);


                Contract contractBuy = new Contract() { ConId = tradePair.ConIdBuy, Exchange = App.EXCHANGE };
                var lmtPrice = Math.Round((double)tradePair.PriceInCentsBuy / 100d, 2);

                Order orderBuy = new Order()
                {
                    OrderId = nextOrderId,
                    Action = "BUY",
                    OrderType = "LIMIT",
                    LmtPrice = lmtPrice,
                    TotalQuantity = tradePair.QuantityBuy
                };

                await _visitor.IbHost.PlaceOrderAsync(nextOrderId, contractBuy, orderBuy, App.TIMEOUT);
            }
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

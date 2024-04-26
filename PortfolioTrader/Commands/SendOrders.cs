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

            foreach(TradePair tradePair in tradePairs)
            {
                var nextOrdeerId = await visitor.IbHost.ReqIdsAsync(-1);


                Contract contractBuy = new Contract() { ConId=tradePair.ConIdBuy, Exchange=App.EXCHANGE};
                //Order orderBuy = new Order()
                //{
                //    OrderId = _nextOrderId,
                //    Action = "BUY",
                //    OrderType = "MARKET",
                //    TotalQuantity = qty
                //}; ;
            }

            //int id = 333; // TODO
            //Contract contract = new Contract();
            //_visitor.IbHost.PlaceOrder
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
                        b.Value.ConId.HasValue ? b.Value.ConId.Value : throw new Exception("Unexpevted. Value is null"),
                        s.Value.ConId.HasValue ? s.Value.ConId.Value : throw new Exception("Unexpevted. Value is null"),
                        b.Value.Quantity.HasValue ? b.Value.Quantity.Value : throw new Exception("Unexpevted. Value is null"),
                        s.Value.Quantity.HasValue ? s.Value.Quantity.Value : throw new Exception("Unexpevted. Value is null")))
                .ToList();

            return tradePairs;

        }
    }
}

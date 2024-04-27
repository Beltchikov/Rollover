﻿using IBApi;
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
                var nextOrderIdBuy = await visitor.IbHost.ReqIdsAsync(-1);
                Contract contractBuy = new Contract() { ConId = tradePair.ConIdBuy, Exchange = App.EXCHANGE };
                var lmtPriceBuy = Math.Round((double)tradePair.PriceInCentsBuy / 100d, 2);

                Order orderBuy = new Order()
                {
                    OrderId = nextOrderIdBuy,
                    Action = "BUY",
                    OrderType = "LIMIT",
                    LmtPrice = lmtPriceBuy,
                    TotalQuantity = tradePair.QuantityBuy
                };

                var resultBuy = await _visitor.IbHost.PlaceOrderAsync(nextOrderIdBuy, contractBuy, orderBuy, App.TIMEOUT);
                visitor.TwsMessageCollection.Add($"orderId={nextOrderIdBuy} Order state: {resultBuy.OrderState}  error:{resultBuy.ErrorMessage}");

                Thread.Sleep(App.TIMEOUT * 2);
            }

            Thread.Sleep(App.TIMEOUT * 2);

            // sell
            foreach (TradePair tradePair in tradePairs)
            {
                var nextOrderIdSell = await visitor.IbHost.ReqIdsAsync(-1);
                Contract contractSell = new Contract() { ConId = tradePair.ConIdSell, Exchange = App.EXCHANGE };
                var lmtPriceSell = Math.Round((double)tradePair.PriceInCentsSell / 100d, 2);

                Order orderSell = new Order()
                {
                    OrderId = nextOrderIdSell,
                    Action = "SELL",
                    OrderType = "LIMIT",
                    LmtPrice = lmtPriceSell,
                    TotalQuantity = tradePair.QuantitySell
                };

                var resultSell = await _visitor.IbHost.PlaceOrderAsync(nextOrderIdSell, contractSell, orderSell, App.TIMEOUT);
                visitor.TwsMessageCollection.Add($"orderId={nextOrderIdSell} Order state: {resultSell.OrderState}  error:{resultSell.ErrorMessage}");

                Thread.Sleep(App.TIMEOUT * 2);
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
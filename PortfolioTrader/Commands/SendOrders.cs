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
                Contract contractBuy = new() { ConId = tradePair.ConIdBuy, Exchange = App.EXCHANGE };
                var lmtPriceBuy = Math.Round((double)tradePair.PriceInCentsBuy / 100d, 2);

                Order orderBuy = new Order()
                {
                    Action = "BUY",
                    OrderType = "LIMIT",
                    LmtPrice = lmtPriceBuy,
                    TotalQuantity = tradePair.QuantityBuy
                };

                var resultBuy = await _visitor.IbHost.PlaceOrderAsync(contractBuy, orderBuy, App.TIMEOUT);
                if (resultBuy.ErrorMessage != "")
                {
                    visitor.TwsMessageCollection.Add($"ConId={tradePair.ConIdBuy} error:{resultBuy.ErrorMessage}");
                    visitor.OrdersLongWithError = SymbolsAndScore.ConcatStringsWithNewLine(
                        visitor.OrdersLongWithError,
                        tradePair.SymbolBuy + " " + resultBuy.ErrorMessage);
                }
                else if (resultBuy.OrderState != null)
                {
                    visitor.TwsMessageCollection.Add($"ConId={tradePair.ConIdBuy} {tradePair.SymbolBuy} order submitted.");
                }
                else throw new Exception("Unexpected. Both ErrorMessage nad OrderState are not set.");

                await Task.Run(() => Thread.Sleep(App.TIMEOUT));

                // sell
                var nextOrderIdSell = await visitor.IbHost.ReqIdsAsync(-1);
                Contract contractSell = new Contract() { ConId = tradePair.ConIdSell, Exchange = App.EXCHANGE };
                var lmtPriceSell = Math.Round((double)tradePair.PriceInCentsSell / 100d, 2);

                Order orderSell = new()
                {
                    Action = "SELL",
                    OrderType = "LIMIT",
                    LmtPrice = lmtPriceSell,
                    TotalQuantity = tradePair.QuantitySell
                };

                var resultSell = await _visitor.IbHost.PlaceOrderAsync(contractSell, orderSell, App.TIMEOUT);
                if (resultSell.ErrorMessage != "")
                {
                    visitor.TwsMessageCollection.Add($"ConId={tradePair.ConIdSell} error:{resultSell.ErrorMessage}");
                    visitor.OrdersShortWithError = SymbolsAndScore.ConcatStringsWithNewLine(
                        visitor.OrdersShortWithError,
                        tradePair.SymbolSell + " " + resultSell.ErrorMessage);
                }
                else if (resultSell.OrderState != null)
                {
                    visitor.TwsMessageCollection.Add($"ConId={tradePair.ConIdSell} {tradePair.SymbolSell} order submitted.");
                }
                else throw new Exception("Unexpected. Both ErrorMessage nad OrderState are not set.");

                await Task.Run(() => Thread.Sleep(App.TIMEOUT));
            }

           _visitor.TwsMessageCollection.Add($"DONE! Send Orders command executed.");
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

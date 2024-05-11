using IBApi;
using IbClient.Types;
using PortfolioTrader.Model;
using System.Windows;

namespace PortfolioTrader.Commands
{
    internal class SendLimitOrders
    {
        private static IBuyConfirmationModelVisitor _visitor = null!;

        public static async Task RunAsync(IBuyConfirmationModelVisitor visitor)
        {
            if (MessageBox.Show(
                "LIMIT orders will be sent now to the broker. Proceed?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.No)
                return;

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

            // Version with Pair Orders

            //_visitor = visitor;
            //List<TradePair> tradePairs = BuildTradePairs();

            //foreach (TradePair tradePair in tradePairs)
            //{
            //    Contract contractBuy = new() { ConId = tradePair.ConIdBuy, Exchange = App.EXCHANGE, Symbol = tradePair.SymbolBuy };
            //    var lmtPriceBuy = Math.Round((double)tradePair.PriceInCentsBuy / 100d, 2);
            //    Contract contractSell = new() { ConId = tradePair.ConIdSell, Exchange = App.EXCHANGE, Symbol = tradePair.SymbolSell };
            //    var lmtPriceSell = Math.Round((double)tradePair.PriceInCentsSell / 100d, 2);

            //    await SendPairOrderAndProcessResultsAsync(
            //        contractBuy: contractBuy,
            //        lmtPriceBuy: lmtPriceBuy,
            //        quantityBuy: tradePair.QuantityBuy,
            //        contractSell: contractSell,
            //        lmtPriceSell: lmtPriceSell,
            //        quantitySell: tradePair.QuantitySell,
            //        visitor);
            //}

            //_visitor.TwsMessageCollection.Add($"DONE! Send Orders command executed.");
        }

        private static async Task SendPairOrderAndProcessResultsAsync(
            Contract contractBuy,
            double lmtPriceBuy,
            int quantityBuy,
            Contract contractSell,
            double lmtPriceSell,
            int quantitySell,
            IBuyConfirmationModelVisitor visitor)
        {
            Order parentOrder = new()
            {
                Action = "BUY",
                OrderType = "LIMIT",
                LmtPrice = lmtPriceBuy,
                TotalQuantity = quantityBuy,
                Transmit = false
            };

            Order hedgeOrder = new()
            {
                Action = "SELL",
                OrderType = "LIMIT",
                LmtPrice = lmtPriceSell,
                TotalQuantity = quantitySell
            };

            var resultParent = await _visitor.IbHost.PlaceOrderAsync(contractBuy, parentOrder, App.TIMEOUT);
            LogResults(result: resultParent, contract: contractBuy, visitor);
            await Task.Run(() => Thread.Sleep(App.TIMEOUT));

            var resultHedge = await _visitor.IbHost.PlaceOrderAsync(contractSell, hedgeOrder, App.TIMEOUT);
            LogResults(result: resultHedge, contract: contractSell, visitor);
            await Task.Run(() => Thread.Sleep(App.TIMEOUT));
        }

        private static void LogResults(OrderStateOrError result, Contract contract, IBuyConfirmationModelVisitor visitor)
        {
            if (result.ErrorMessage != "")
            {
                var log = $"Error sending order for parent {contract.Symbol} vonId={contract.ConId} error:{result.ErrorMessage}";
                visitor.TwsMessageCollection.Add(log);
                visitor.OrdersLongWithError = SymbolsAndScore.ConcatStringsWithNewLine(
                    visitor.OrdersLongWithError,
                    contract.Symbol + " " + result.ErrorMessage);
            }
            else if (result.OrderState != null)
            {
                visitor.TwsMessageCollection.Add($"{contract.Symbol} conId={contract.ConId} order submitted.");
            }
            else throw new Exception("Unexpected. Both ErrorMessage nad OrderState are not set.");
        }

        private static async Task SendOrderAndProcessResultsAsync(bool isLong, Contract contract, double lmtPrice, int quantity, IBuyConfirmationModelVisitor visitor)
        {
            Order order = new()
            {
                Action = isLong ? "BUY" : "SELL",
                OrderType = "LIMIT",
                LmtPrice = lmtPrice,
                TotalQuantity = quantity
            };

            var result = await _visitor.IbHost.PlaceOrderAsync(contract, order, App.TIMEOUT);
            if (result.ErrorMessage != "")
            {
                var log = $"Error sending order for {contract.Symbol} vonId={contract.ConId} error:{result.ErrorMessage}";
                visitor.TwsMessageCollection.Add(log);
                if (isLong) visitor.OrdersLongWithError = SymbolsAndScore.ConcatStringsWithNewLine(
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

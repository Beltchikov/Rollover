using IBApi;
using PortfolioTrader.Algos;
using PortfolioTrader.Model;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PortfolioTrader.Commands
{
    internal class SendBracketOrders
    {
        private static readonly string FORMAT_STRING_UI = "dd.MM.yyyy HH:mm:ss";
        private static readonly string FORMAT_STRING_API = "yyyyMMdd-HH:mm:ss";

        public static async Task RunAsync(IBuyConfirmationModelVisitor visitor)
        {
            if (visitor.EntryBarTime > DateTime.Now)
            {
                MessageBox.Show("The time of entry bar is in the future. Please correct the time. The execution stops.");
                return;
            }

            (int hoursUtcOffset, int minutesUtcOffset) = HoursAndMinutesFromUtcOffset(visitor.UtcOffset);
            var localTime = visitor.EntryBarTime
                .AddHours(hoursUtcOffset)
                .AddMinutes(minutesUtcOffset);

            if (MessageBox.Show(
                $"STOP LIMIT orders will be sent now to the broker. Time of entry bar: {localTime.ToString(FORMAT_STRING_UI)} Proceed?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            List<TradePair> tradePairs = BuildTradePairs(visitor);

            string endDateTime = visitor.EntryBarTime.ToString(FORMAT_STRING_API);
            string durationString = "300 S";
            string barSizeSetting = "5 mins";
            string whatToShow = "TRADES";
            int useRTH = 0;
            bool keepUpToDate = false;

            // buy
            foreach (TradePair tradePair in tradePairs)
            {
                // Buy
                Contract contractBuy = new()
                {
                    ConId = tradePair.ConIdBuy,
                    Symbol = tradePair.SymbolBuy,
                    SecType = App.SEC_TYPE_STK,
                    Currency = App.USD,
                    Exchange = App.EXCHANGE
                };
                double entryStpPriceBuy = 0;
                double slStpPriceBuy = 0;

                await visitor.IbHost.RequestHistoricalDataAsync(
                    contractBuy,
                    endDateTime,
                    durationString,
                    barSizeSetting,
                    whatToShow,
                    useRTH,
                    1,
                    keepUpToDate,
                    [],
                    App.TIMEOUT,
                    (d) => { entryStpPriceBuy = d.High + 0.01; slStpPriceBuy = d.Low - 0.01; },
                    (u) => { },
                (e) => { });

                double entryLmtPriceBuy = LimitPrice.PercentageOfPriceOrFixed(isLong: true, entryStpPriceBuy);
                double slLmtPriceBuy = LimitPrice.PercentageOfPriceOrFixed(isLong: false, slStpPriceBuy);
                (Order orderBuyParent, Order orderBuyStop) = CreateOrders(
                    isLong: true,
                    tradePair,
                    entryStpPrice: entryStpPriceBuy,
                    entryLmtPrice: entryLmtPriceBuy,
                    slStpPrice: slStpPriceBuy,
                    slLmtPrice: slLmtPriceBuy);

                await SendOrders(isLong: true, visitor, tradePair, contractBuy, orderBuyParent);
                await SendOrders(isLong: false, visitor, tradePair, contractBuy, orderBuyStop);
                await Task.Run(() => Thread.Sleep(App.TIMEOUT));

                // sell
                Contract contractSell = new Contract()
                {
                    ConId = tradePair.ConIdSell,
                    Symbol = tradePair.SymbolSell,
                    SecType = App.SEC_TYPE_STK,
                    Currency = App.USD,
                    Exchange = App.EXCHANGE
                };

                double entryStpPriceSell = 0;
                double slStpPriceSell = 0;

                await visitor.IbHost.RequestHistoricalDataAsync(
                    contractSell,
                    endDateTime,
                    durationString,
                    barSizeSetting,
                    whatToShow,
                    useRTH,
                    1,
                    keepUpToDate,
                    [],
                    App.TIMEOUT,
                    (d) => { entryStpPriceSell = d.Low - 0.01; slStpPriceSell = d.High + 0.01; },
                    (u) => { },
                    (e) => { });

                double entryLmtPriceSell = LimitPrice.PercentageOfPriceOrFixed(isLong: false, entryStpPriceSell);
                double slLmtPriceSell = LimitPrice.PercentageOfPriceOrFixed(isLong: true, slStpPriceSell);
                (Order orderSellParent, Order orderSellStop) = CreateOrders(
                    isLong: false,
                    tradePair,
                    entryStpPrice: entryStpPriceSell,
                    entryLmtPrice: entryLmtPriceSell,
                    slStpPrice: slStpPriceSell,
                    slLmtPrice: slLmtPriceSell);

                await SendOrders(isLong: false, visitor, tradePair, contractSell, orderSellParent);
                await SendOrders(isLong: true, visitor, tradePair, contractSell, orderSellStop);
                await Task.Run(() => Thread.Sleep(App.TIMEOUT));
            }

            visitor.TwsMessageCollection.Add($"DONE! Send Orders command executed.");

        }

        private static async Task SendOrders(bool isLong, IBuyConfirmationModelVisitor visitor, TradePair tradePair, Contract contract, Order orderParent)
        {
            var result = await visitor.IbHost.PlaceOrderAsync(contract, orderParent, App.TIMEOUT);

            var conId = isLong ? tradePair.ConIdBuy : tradePair.ConIdSell;
            var symbol = isLong ? tradePair.SymbolBuy : tradePair.SymbolSell;
            if (result.ErrorMessage != "")
            {
                visitor.TwsMessageCollection.Add($"ConId={conId} error:{result.ErrorMessage}");
                if(isLong)
                {
                    visitor.OrdersLongWithError = SymbolsAndScore.ConcatStringsWithNewLine(
                    visitor.OrdersLongWithError,
                    symbol + " " + result.ErrorMessage);
                }
                else
                {
                    visitor.OrdersShortWithError = SymbolsAndScore.ConcatStringsWithNewLine(
                    visitor.OrdersShortWithError,
                    symbol + " " + result.ErrorMessage);
                }
            }
            else if (result.OrderState != null)
            {
                visitor.TwsMessageCollection.Add($"ConId={conId} {symbol} order submitted.");
            }
            else throw new Exception("Unexpected. Both ErrorMessage nad OrderState are not set.");
        }

        private static (Order, Order) CreateOrders(
            bool isLong,
            TradePair tradePair,
            double entryStpPrice,
            double entryLmtPrice,
            double slStpPrice,
            double slLmtPrice)
        {
            var orderParent = new Order()
            {
                Action = isLong ? "BUY" : "SELL",
                OrderType = "STP LMT",
                AuxPrice = entryStpPrice,
                LmtPrice = entryLmtPrice,
                TotalQuantity = isLong ? tradePair.QuantityBuy : tradePair.QuantitySell,
                Transmit = false
            };

            Order orderStop = new()
            {
                OrderId = orderParent.OrderId + 1,
                Action = isLong ? "SELL" : "BUY",
                OrderType = "MIDPRICE",
                AuxPrice = slLmtPrice,
                TotalQuantity = isLong ? tradePair.QuantityBuy : tradePair.QuantitySell,
                ParentId = orderParent.OrderId,
                Transmit = true
            };

            //PriceCondition priceCondition = (PriceCondition)OrderCondition.Create(OrderConditionType.Price);
            ////When this contract...
            //priceCondition.ConId = conId;
            ////traded on this exchange
            //priceCondition.Exchange = exchange;
            ////has a price above/below
            //priceCondition.IsMore = isMore;
            ////this quantity
            //priceCondition.Price = price;
            ////AND | OR next condition (will be ignored if no more conditions are added)
            //priceCondition.IsConjunctionConnection = isConjunction;



            return (orderParent, orderStop);
        }

        private static (int hoursUtcOffset, int minutesUtcOffset) HoursAndMinutesFromUtcOffset(string utcOffset)
        {
            int sign = 1;
            if (utcOffset.StartsWith('-'))
            {
                sign = -1;
            }

            var splitted = utcOffset.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var hours = int.Parse(splitted[0]);
            var minutes = int.Parse(splitted[1]);

            return (hours * sign, minutes * sign);
        }

        private static List<TradePair> BuildTradePairs(IBuyConfirmationModelVisitor visitor)
        {
            var buyDictionary = SymbolsAndScore.StringToPositionDictionary(visitor.StocksToBuyAsString);
            var sellDictionary = SymbolsAndScore.StringToPositionDictionary(visitor.StocksToSellAsString);

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

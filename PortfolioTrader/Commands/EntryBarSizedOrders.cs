﻿using IBApi;
using PortfolioTrader.Algos;
using PortfolioTrader.Model;
using System.Windows;

namespace PortfolioTrader.Commands
{
    internal class EntryBarSizedOrders
    {
        private static readonly string FORMAT_STRING_UI = "dd.MM.yyyy HH:mm:ss";
        private static readonly string FORMAT_STRING_API = "yyyyMMdd-HH:mm:ss";
        private static int _lastOrderId;
        private static int _nextOrderId;

        public static async Task RunAsync(IBuyConfirmationModelVisitor visitor)
        {
            //// TODO
            //MessageBox.Show("EntryBarSizedOrders");
            //return;

            //////////////////
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

            //
            var buyDictionary = SymbolsAndScore.StringToPositionDictionary(visitor.StocksToBuyAsString);
            var sellDictionary = SymbolsAndScore.StringToPositionDictionary(visitor.StocksToSellAsString);

            buyDictionary = await AddBarColumnAsync(visitor, buyDictionary);
            sellDictionary = await AddBarColumnAsync(visitor, sellDictionary);

            buyDictionary = DoRecalculateWeights(visitor, buyDictionary);
            sellDictionary = DoRecalculateWeights(visitor, sellDictionary);

            buyDictionary = Position.CalculateQuantity(buyDictionary, visitor.InvestmentAmount);
            sellDictionary = Position.CalculateQuantity(sellDictionary, visitor.InvestmentAmount);

            visitor.StocksToBuyAsString = SymbolsAndScore.PositionDictionaryToString(buyDictionary);
            visitor.StocksToSellAsString = SymbolsAndScore.PositionDictionaryToString(sellDictionary);


            //
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
                     (d) => {
                         entryStpPriceBuy = Position.RoundPrice(isHighPrice: true, d.High + 0.01);
                         slStpPriceBuy = Position.RoundPrice(isHighPrice: false, d.Low - 0.01);
                     },
                    (u) => { },
                (e) => { });

                // Check
                var barInCentsBuy = Math.Round((entryStpPriceBuy - slStpPriceBuy) * 100, 0);
                if (tradePair.BarInCentsBuy != barInCentsBuy)
                {
                    MessageBox.Show($"Entry bar check for {tradePair.SymbolBuy} is not passed.");
                }
                else
                {
                    double entryLmtPriceBuy = LimitPrice.PercentageOfPriceOrFixed(isLong: true, entryStpPriceBuy);
                    double slLmtPriceBuy = LimitPrice.PercentageOfPriceOrFixed(isLong: false, slStpPriceBuy);
                    (Order orderBuyParent, Order orderBuyStop) = await CreateOrdersAsync(
                        isLong: true,
                        visitor,
                        tradePair,
                        entryStpPrice: entryStpPriceBuy,
                        entryLmtPrice: entryLmtPriceBuy,
                        slStpPrice: slStpPriceBuy,
                        slLmtPrice: slLmtPriceBuy);

                    await SendOrders(isLong: true, visitor, tradePair, contractBuy, orderBuyParent);
                    await SendOrders(isLong: false, visitor, tradePair, contractBuy, orderBuyStop);
                }
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
                     (d) => {
                         slStpPriceSell = Position.RoundPrice(isHighPrice: true, d.High + 0.01);
                         entryStpPriceSell = Position.RoundPrice(isHighPrice: false, d.Low - 0.01);
                     },
                    (u) => { },
                    (e) => { });

                // Check
                var barInCentsSell = Math.Round((slStpPriceSell - entryStpPriceSell) * 100, 0);
                if (tradePair.BarInCentsSell != barInCentsSell)
                {
                    MessageBox.Show($"Entry bar check for {tradePair.SymbolSell} is not passed.");
                }
                else
                {
                    double entryLmtPriceSell = LimitPrice.PercentageOfPriceOrFixed(isLong: false, entryStpPriceSell);
                    double slLmtPriceSell = LimitPrice.PercentageOfPriceOrFixed(isLong: true, slStpPriceSell);
                    (Order orderSellParent, Order orderSellStop) = await CreateOrdersAsync(
                        isLong: false,
                        visitor,
                        tradePair,
                        entryStpPrice: entryStpPriceSell,
                        entryLmtPrice: entryLmtPriceSell,
                        slStpPrice: slStpPriceSell,
                        slLmtPrice: slLmtPriceSell);

                    await SendOrders(isLong: false, visitor, tradePair, contractSell, orderSellParent);
                    await SendOrders(isLong: true, visitor, tradePair, contractSell, orderSellStop);
                }
                await Task.Run(() => Thread.Sleep(App.TIMEOUT));
            }

            visitor.TwsMessageCollection.Add($"DONE! Send Orders command executed.");

        }

        private static Dictionary<string, Position> DoRecalculateWeights(IBuyConfirmationModelVisitor visitor, Dictionary<string, Position> dictionary)
        {
            Dictionary<string, Position> resultDictionary = [];

            var scaler = dictionary.Values.Sum(v => v.BarInCents);
            var scalerNotNullable = scaler ?? throw new Exception("Unexpected. scaler is null.");

            List<double> barsReciprocal = dictionary.Values.Select(v =>
                (double)scaler / (v.BarInCents
                    ?? throw new Exception("Unexpected. BarInCents is null."))).ToList();
            var scalerReciprocal = barsReciprocal.Sum();

            List<double> weightBarList = barsReciprocal.Select(b => b / scalerReciprocal).ToList();
            List<int> weightList = dictionary.Values.Select(v => v.Weight ?? throw new Exception("BarInCents is null.")).ToList();
            List<double> weightBmsAndBarNotScaledList = weightBarList.Zip(weightList, (r, n) => r * n).ToList();
            var scalerBmsAndBar = weightBmsAndBarNotScaledList.Sum();
            List<int> weightBarScaledList = weightBmsAndBarNotScaledList.Select(x => (int)Math.Round((x / scalerBmsAndBar) * 100, 0)).ToList();

            int i = 0;
            foreach (var kvp in dictionary)
            {
                kvp.Value.Weight = weightBarScaledList[i];
                resultDictionary.Add(kvp.Key, kvp.Value);
                i++;
            }

            return resultDictionary;
        }

        private static async Task<Dictionary<string, Position>> AddBarColumnAsync(
            IBuyConfirmationModelVisitor visitor,
            Dictionary<string, Position> dictionary)
        {
            Dictionary<string, Position> resultDictionary = [];

            string endDateTime = visitor.EntryBarTime.ToString(FORMAT_STRING_API);
            string durationString = "300 S";
            string barSizeSetting = "5 mins";
            string whatToShow = "TRADES";
            int useRTH = 0;
            bool keepUpToDate = false;

            foreach (var kvp in dictionary)
            {
                var conIdNotNullable = kvp.Value.ConId ?? throw new Exception("Unexpected. kvp.Value.ConId is null.");
                Contract contract = new()
                {
                    ConId = conIdNotNullable,
                    Symbol = kvp.Key,
                    SecType = App.SEC_TYPE_STK,
                    Currency = App.USD,
                    Exchange = App.EXCHANGE
                };

                double barLow = 0;
                double barHigh = 0;

                await visitor.IbHost.RequestHistoricalDataAsync(
                    contract,
                    endDateTime,
                    durationString,
                    barSizeSetting,
                    whatToShow,
                    useRTH,
                    1,
                    keepUpToDate,
                    [],
                    App.TIMEOUT,
                     (d) => {
                         barHigh = Position.RoundPrice(isHighPrice: true, d.High + 0.01);
                         barLow = Position.RoundPrice(isHighPrice: false, d.Low - 0.01);
                     },
                    (u) => { },
                    (e) => { });

                var barInCents = (int)Math.Round((barHigh - barLow) * 100, 0);
                kvp.Value.BarInCents = barInCents;
                resultDictionary.Add(kvp.Key, kvp.Value);
            }

            return resultDictionary;
        }

        private static async Task SendOrders(bool isLong, IBuyConfirmationModelVisitor visitor, TradePair tradePair, Contract contract, Order orderParent)
        {
            var result = await visitor.IbHost.PlaceOrderAsync(contract, orderParent, App.TIMEOUT);

            var conId = isLong ? tradePair.ConIdBuy : tradePair.ConIdSell;
            var symbol = isLong ? tradePair.SymbolBuy : tradePair.SymbolSell;
            if (result.ErrorMessage != "")
            {
                visitor.TwsMessageCollection.Add($"ConId={conId} error:{result.ErrorMessage}");
                if (isLong)
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

        private static async Task<(Order, Order)> CreateOrdersAsync(
            bool isLong,
            IBuyConfirmationModelVisitor visitor,
            TradePair tradePair,
            double entryStpPrice,
            double entryLmtPrice,
            double slStpPrice,
            double slLmtPrice)
        {

            var orderBuyId = await visitor.IbHost.ReqIdsAsync(-1);
            var orderParent = new Order()
            {
                OrderId = orderBuyId,
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
                LmtPrice = slLmtPrice,
                TotalQuantity = isLong ? tradePair.QuantityBuy : tradePair.QuantitySell,
                ParentId = orderParent.OrderId,
                Transmit = true
            };

            PriceCondition priceCondition = (PriceCondition)OrderCondition.Create(OrderConditionType.Price);
            priceCondition.ConId = isLong ? tradePair.ConIdBuy : tradePair.ConIdSell;
            priceCondition.Exchange = App.EXCHANGE;
            priceCondition.IsMore = !isLong;
            priceCondition.Price = slStpPrice;

            orderStop.Conditions.Add(priceCondition);

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
                        s.Value.Quantity.HasValue ? s.Value.Quantity.Value : throw new Exception("Unexpected. Value is null"),
                        b.Value.BarInCents.HasValue ? b.Value.BarInCents.Value : throw new Exception("Unexpected. Value is null"),
                        s.Value.BarInCents.HasValue ? s.Value.BarInCents.Value : throw new Exception("Unexpected. Value is null")))
                .ToList();

            return tradePairs;

        }
    }
}

using IBApi;
using IbClient.Types;
using PortfolioTrader.Algos;
using PortfolioTrader.Model;
using System.Windows;

namespace PortfolioTrader.Commands
{
    internal class SendStopLimitOrders
    {
        private static readonly string  FORMAT_STRING_UI = "dd.MM.yyyy HH:mm:ss";
        private static readonly string  FORMAT_STRING_API = "yyyyMMdd-HH:mm:ss";
        private static int _lastOrderId;
        private static int _nextOrderId;

        public static async Task RunAsync(IBuyConfirmationModelVisitor visitor)
        {
            if(visitor.EntryBarTime > DateTime.Now)
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
                Contract contractBuy = new() { 
                    ConId = tradePair.ConIdBuy, 
                    Symbol = tradePair.SymbolBuy,   
                    SecType = App.SEC_TYPE_STK,
                    Currency = App.USD,
                    Exchange = App.EXCHANGE 
                };
                double auxPriceBuy = 0;
                
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
                    (d) => auxPriceBuy = d.High + 0.01,
                    (u) => { },
                    (e) => { });

                Order orderBuy = new Order()
                {
                    Action = "BUY",
                    OrderType = "STP LMT",
                    AuxPrice = auxPriceBuy,
                    LmtPrice = LimitPrice.PercentageOfPriceOrFixed(isLong: true, auxPriceBuy),
                    TotalQuantity = tradePair.QuantityBuy
                };

                orderBuy.OrderId = await visitor.IbHost.ReqIdsAsync(-1);

                var resultBuy = await visitor.IbHost.PlaceOrderAsync(contractBuy, orderBuy, App.TIMEOUT);
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
                Contract contractSell = new Contract() { 
                    ConId = tradePair.ConIdSell, 
                    Symbol = tradePair.SymbolSell,
                    SecType = App.SEC_TYPE_STK,
                    Currency = App.USD,
                    Exchange = App.EXCHANGE 
                };

                double auxPriceSell = 0;

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
                    (d) => auxPriceSell = d.Low - 0.01,
                    (u) => { },
                    (e) => { });

                Order orderSell = new()
                {
                    Action = "SELL",
                    OrderType = "STP LMT",
                    AuxPrice = auxPriceSell,
                    LmtPrice = LimitPrice.PercentageOfPriceOrFixed(isLong: false, auxPriceSell),
                    TotalQuantity = tradePair.QuantitySell
                };

                orderSell.OrderId = await visitor.IbHost.ReqIdsAsync(-1);

                var resultSell = await visitor.IbHost.PlaceOrderAsync(contractSell, orderSell, App.TIMEOUT);
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

            visitor.TwsMessageCollection.Add($"DONE! Send Orders command executed.");

        }

        private static (int hoursUtcOffset, int minutesUtcOffset) HoursAndMinutesFromUtcOffset(string utcOffset)
        {
            int sign = 1;
            if(utcOffset.StartsWith('-'))
            {
                sign = -1;
            }

            var splitted = utcOffset.Split(':', StringSplitOptions.RemoveEmptyEntries);    
            var hours = int.Parse(splitted[0]);
            var minutes = int.Parse(splitted[1]);

            return(hours * sign, minutes * sign); 
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
                        0,
                        0))
                .ToList();

            return tradePairs;

        }
    }
}

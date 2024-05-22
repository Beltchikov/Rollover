using IBApi;
using PortfolioTrader.Model;
using System.Windows;

namespace PortfolioTrader.Commands
{
    [Obsolete]
    internal class RecalculateWeights
    {
        private static readonly string FORMAT_STRING_API = "yyyyMMdd-HH:mm:ss";

        public static async Task RunAsync(IBuyConfirmationModelVisitor visitor)
        {
            if (visitor.EntryBarTime > DateTime.Now)
            {
                MessageBox.Show("The time of entry bar is in the future. Please correct the time. The execution stops.");
                return;
            }

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

            var msg = $"DONE! Recalculate weights command executed.";
            visitor.TwsMessageCollection.Add(msg);
            MessageBox.Show(msg);
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

                await visitor.IbHost.RequestHistoricalAndSubscribeAsync(
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
                    (d) => { barLow = d.Low; barHigh = d.High; },
                    (u) => { },
                    (e) => { });

                var barInCents = (int)Math.Round((barHigh - barLow) * 100, 0);
                kvp.Value.BarInCents = barInCents;
                resultDictionary.Add(kvp.Key, kvp.Value);
            }

            return resultDictionary;
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
    }
}

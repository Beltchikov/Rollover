using System.Globalization;
using Ta;

namespace SignalAdvisor.Commands
{
    class RequestHistoricalData
    {
        static readonly int BAR_SIZE = 5;

        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            var now = DateTime.Now;
            var historicalDataStart = now.AddHours(-1 * App.HISTORICAL_DATA_PERIOD_IN_HOURS);
            string durationString = $"{(now - historicalDataStart).Days} D";
            string barSizeSetting = $"{BAR_SIZE} mins";
            string whatToShow = "TRADES";
            int useRTH = 0;
            bool keepUpToDate = true;

            foreach (var positionMessage in visitor.Positions)
            {
                var contractSmartExchange = new IBApi.Contract()
                {
                    SecType = positionMessage.Contract.SecType,
                    Symbol = positionMessage.Contract.Symbol,
                    Currency = positionMessage.Contract.Currency,
                    Exchange = App.EXCHANGE
                };
                var contractOriginalString = positionMessage.Contract.ToString();

                bool historicalDataReceived = false;
                await visitor.IbHost.RequestHistoricalDataAsync(
                    contractSmartExchange,
                    "",
                    durationString,
                    barSizeSetting,
                    whatToShow,
                    useRTH,
                    1,
                    keepUpToDate,
                    [],
                    App.TIMEOUT,
                    (d) =>
                    {
                        var bar = new Bar(d.Open, d.High, d.Low, d.Close, TimeFromString(d.Date));
                        if (!visitor.Bars.Any(kvp => kvp.Key == contractOriginalString))
                            visitor.Bars.Add(new KeyValuePair<string, List<Bar>>(contractOriginalString, []));
                        visitor.Bars.First(kvp => kvp.Key == contractOriginalString).Value.Add(bar);
                    },
                    (u) => {
                        //var time = TimeFromString(u.Date);
                        //if(time.Minute/ BAR_SIZE)
                    },
                    (e) => { historicalDataReceived = true; });

                await Task.Run(() => { while (!historicalDataReceived) { }; });
            }
        }

        private static DateTime TimeFromString(string dateTimeString)
        {
            var timeString = dateTimeString
                         .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                         .Select(s => s.Trim())
                         .ToArray()
                         .Aggregate((r, n) => r + " " + n);

            return DateTime.ParseExact(timeString, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}

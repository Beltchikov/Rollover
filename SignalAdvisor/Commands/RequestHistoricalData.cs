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
            int useRTH = 1;
            bool keepUpToDate = true;

            foreach (var positionMessage in visitor.Positions)
            {
                var contract = new IBApi.Contract()
                {
                    SecType = positionMessage.Contract.SecType,
                    Symbol = positionMessage.Contract.Symbol,
                    Currency = positionMessage.Contract.Currency,
                    Exchange = App.EXCHANGE
                };

                bool historicalDataReceived = false;
                await visitor.IbHost.RequestHistoricalDataAsync(
                    contract,
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
                        var contractString = positionMessage.Contract.ToString();
                        var time = DateTime.Parse(d.Date);
                        var bar = new Bar(d.Open, d.High, d.Low, d.Close, time);

                        if (!visitor.Bars.Any(kvp => kvp.Key == contractString))
                            visitor.Bars.Add(new KeyValuePair<string, List<Bar>>(contractString, []));
                        visitor.Bars.First(kvp => kvp.Key == contractString).Value.Add(bar);
                    },
                    (u) => { var todo = 0; },
                    (e) => { historicalDataReceived = true; });

                await Task.Run(() => { while (!historicalDataReceived) { }; });
            }
        }
    }
}

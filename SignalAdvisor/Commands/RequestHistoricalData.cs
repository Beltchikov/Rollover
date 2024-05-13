using IBApi;

namespace SignalAdvisor.Commands
{
    class RequestHistoricalData
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            var now = DateTime.Now;
            var historicalDataStart = now.AddHours(-1 * App.HISTORICAL_DATA_PERIOD_IN_HOURS);
            string endDateTime = now.ToString("yyyyMMdd HH:mm:ss");
            //string durationString = "300 S";
            string durationString = $"{(now-historicalDataStart).TotalSeconds} S";
            string barSizeSetting = "5 mins";
            string whatToShow = "TRADES";
            int useRTH = 0;
            bool keepUpToDate = true;

            // TODO

            foreach (var position in visitor.Positions)
            {
                bool historicalDataReceived = false;

                var conId = 0; // TODO

                Contract contractBuy = new() { ConId = conId, Exchange = App.EXCHANGE };
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

                await Task.Run(() => { while (!historicalDataReceived) { }; });
            }
        }
    }
}

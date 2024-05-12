using IBApi;

namespace SignalAdvisor.Commands
{
    class RequestHistoricalData
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            string endDateTime = ""; // TODO
            string durationString = "300 S";
            string barSizeSetting = "5 mins";
            string whatToShow = "TRADES";
            int useRTH = 0;
            bool keepUpToDate = false;

            // TODO
            
            //foreach (var position in visitor.Positions)
            //{
            //    bool positionsRequested = false;

            //    var conId = 0; // TODO

            //    Contract contractBuy = new() { ConId = conId, Exchange = App.EXCHANGE };
            //    double auxPriceBuy = 0;

            //    await visitor.IbHost.RequestHistoricalDataAsync(
            //        contractBuy,
            //        endDateTime,
            //        durationString,
            //        barSizeSetting,
            //        whatToShow,
            //        useRTH,
            //        1,
            //        keepUpToDate,
            //        [],
            //        App.TIMEOUT,
            //        (d) => auxPriceBuy = d.High + 0.01,
            //        (u) => { },
            //        (e) => { });

            //    await Task.Run(() => { while (!positionsRequested) { }; });
            //}
        }
    }
}

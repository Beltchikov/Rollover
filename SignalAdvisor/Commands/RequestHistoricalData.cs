using SignalAdvisor.Extensions;
using System.IO;

namespace SignalAdvisor.Commands
{
    class RequestHistoricalData
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            var now = DateTime.Now;
            var historicalDataStart = now.AddHours(-1 * App.HISTORICAL_DATA_PERIOD_IN_HOURS);
            string durationString = $"{(now - historicalDataStart).Days} D";
            string whatToShow = "TRADES";
            int useRTH = 0;

            foreach (var instrument in visitor.Instruments)
            {
                bool historicalDataReceived = false;
                var contract = instrument.ToContract();
                var contractString = contract.ToString();

                await visitor.IbHost.RequestHistoricalAndSubscribeAsync(
                    contract,
                    "",
                    durationString,
                    App.BAR_SIZE,
                    whatToShow,
                    useRTH,
                    1,
                    [],
                    App.TIMEOUT,
                    (historicalDataMessage) => // Historical Data Callback
                    {
                        // Add bar logic
                        var barTime = historicalDataMessage.Date.DateTimeOffsetFromString();
                        var signals = visitor.Signals.For(contractString);
                        if (!visitor.Bars.For(contractString).Any())
                        {
                            visitor.AddBar(contract, historicalDataMessage);
                        }

                        var lastBar = visitor.Bars.For(contractString).Last();
                        var lastBarTime = lastBar.Time;

                        if (barTime.Minute / 5 != lastBarTime.Minute / 5)
                            visitor.AddBar(contract, historicalDataMessage);
                        else
                            lastBar.Update(historicalDataMessage.High, historicalDataMessage.Low, historicalDataMessage.Close);
                    },
                    (historicalDataMessage) => // Historical Data UPDATE Callback
                    {
                        visitor.AddBar(contract, historicalDataMessage);
                    },
                    (e) => { historicalDataReceived = true; }); // Historical Data END Calllback

                await Task.Run(() => { while (!historicalDataReceived) { }; });

                // Save historical data
                var fileName = contractString.Replace(" ", "-")+ ".csv";
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                string barsAsString = visitor.Bars.For(contractString).Select(b=>b.ToCsvString(";")).Aggregate((r,n)=> r+ Environment.NewLine + n);
                File.WriteAllText(filePath, barsAsString);
            }
            
            visitor.RequestHistoricalDataExecuted = true;
        }
    }
}

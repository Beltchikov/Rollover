using SignalAdvisor.Extensions;
using System.IO;
using Ta;

namespace SignalAdvisor.Commands
{
    class RequestHistoricalData
    {
        static readonly int BAR_SIZE = 5;
        static private readonly string HISTORICAL_DATA_PATH = "HistoricalData\\";

        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            var now = DateTime.Now;
            var historicalDataStart = now.AddHours(-1 * App.HISTORICAL_DATA_PERIOD_IN_HOURS);
            string durationString = $"{(now - historicalDataStart).Days} D";
            string barSizeSetting = $"{BAR_SIZE} mins";
            string whatToShow = "TRADES";
            int useRTH = 0;

            foreach (var positionMessage in visitor.Positions)
            {
                if(positionMessage.Contract.SecType == "OPT")
                {
                    continue; // Not implemented yet for options.
                }
                
                bool historicalDataReceived = false;
                List<Bar> bars = null!;
                var contractString = positionMessage.Contract.ToString();

                await visitor.IbHost.RequestHistoricalAndSubscribeAsync(
                    positionMessage.Contract,
                    "",
                    durationString,
                    barSizeSetting,
                    whatToShow,
                    useRTH,
                    1,
                    [],
                    App.TIMEOUT,
                    (d) =>
                    {
                        // Add bar logic
                        var barTime = d.Date.DateTimeOffsetFromString();

                        bars = visitor.Bars.For(contractString);
                        var signals = visitor.Signals.For(contractString);
                        if (!bars.Any())
                        {
                            var bar0 = new Bar(d.Open, d.High, d.Low, d.Close, d.Date.DateTimeOffsetFromString());
                            bars.Add(bar0);

                            var bar1 = bars.SkipLast(1).LastOrDefault();
                            //if (bar1 != null)
                            //{
                            //    signals.Add(Signals.OppositeColor(bar0, bar1));
                            //}

                            return;
                        }

                        var lastBar = bars.Last();
                        var lastBarTime = lastBar.Time;

                        if (barTime.Minute / 5 != lastBarTime.Minute / 5)
                            visitor.AddBar(positionMessage.Contract, d);
                        else
                            lastBar.Update(d.High, d.Low, d.Close);
                    },
                    (u) =>
                    {
                        visitor.AddBar(positionMessage.Contract, u);
                        //var time = TimeFromString(u.Date);
                        //if(time.Minute/ BAR_SIZE)
                    },
                    (e) => { historicalDataReceived = true; });

                await Task.Run(() => { while (!historicalDataReceived) { }; });

                // Save historical data
                var fileName = contractString.Replace(" ", "-")+ ".csv";
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                string barsAsString = bars.Select(b=>b.ToCsvString()).Aggregate((r,n)=> r+ Environment.NewLine + n);
                File.WriteAllText(filePath, barsAsString);
            }
            
            visitor.RequestHistoricalDataExecuted = true;
        }
    }
}

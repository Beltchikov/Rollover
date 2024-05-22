﻿using SignalAdvisor.Extensions;
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
            
            foreach (var positionMessage in visitor.Positions)
            {
                bool historicalDataReceived = false;
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

                        var bars = visitor.Bars.For(positionMessage.Contract.ToString());
                        if (!bars.Any())
                        {
                            bars.Add(new Bar(d.Open, d.High, d.Low, d.Close, d.Date.DateTimeOffsetFromString()));
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
                    (e) => { historicalDataReceived = true;});

                await Task.Run(() => { while (!historicalDataReceived) { }; });
            }
            visitor.RequestHistoricalDataExecuted = true;
        }
    }
}

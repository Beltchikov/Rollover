﻿using SignalAdvisor.Extensions;
using System.IO;

namespace SignalAdvisor.Commands
{
    class RequestHistoricalData
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            foreach (var instrument in visitor.Instruments)
            {
                bool historicalDataReceived = false;
                var contract = instrument.ToContract();
                var contractString = contract.ToString();

                await visitor.IbHost.RequestHistoricalAndSubscribeAsync(
                    contract,
                    "",
                    App.DURATION_STRING,
                    App.BAR_SIZE,
                    "TRADES",
                    0,
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

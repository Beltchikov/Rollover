﻿using IBSampleApp.messages;
using System.Media;

namespace SignalAdvisor.Commands
{
    public class StartMonitoring
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            UpdateItems.Run(visitor);

            await RequestHistoricalData.RunAsync(visitor);
            await Task.Run(() => { while (!visitor.RequestHistoricalDataExecuted) { } });

            TriggerAlert(visitor);

            //foreach (var instrument in visitor.Instruments)
            //{

            //Macd macd = new Macd(12,26,9, 235.318, 235.233, 0.125);

            // var historicalDatapoints = new DataPoints();
            //await RequestHistoricalDataAndSubscribe.RunAsync(
            //  instrument,
            //  historicalDataMessage => { historicalDatapoints.AddDataPoint(historicalDataMessage.Time, historicalDataMessage.Close) },
            //  historicalDataEndMessage => {
            //  macd.AddDataPoints(historicalDatapoints);
            //  visitor.RequestHistoricalDataExecuted = true;}
            //  updateMessage => {
            //      macd.AddDataPoint(updateMessage.Time, updateMessage.Close);
            //      if(macd.NewBar(0) && macdCrossUp(1))  TriggerAlert(visitor);
            //  });

            // await Task.Run(() => { while (!visitor.RequestHistoricalDataExecuted) { } });

            // DataPoint lastMacdCrossUp = macd.LastCrossUp();
            // instrument.LastSignalTime = lastMacdCrossUp.Time;

            //}

        }

        private static void TriggerAlert(IPositionsVisitor visitor)
        {
            _ = Task.Run(() =>
            {
                SoundPlayer player = new(Properties.Resources.Alarm01_wav);
                while (!visitor.AlertDeactivated) { player.PlaySync(); }
            });
        }
    }
}

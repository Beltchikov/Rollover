using IBSampleApp.messages;
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

            _ = Task.Run(() =>
            {
                SoundPlayer player = new(Properties.Resources.Alarm01_wav);
                while (!visitor.AlertDeactivated) { player.PlaySync(); }
            });

            //foreach (var instrument in visitor.Instruments)
            //{

            //Macd macd = new Macd(12,26,9);

            //await RequestHistoricalDataAndSubscribe.RunAsync(
            //  instrument,
            //  historicalDataMessage => { macd.AddDataPoint(historicalDataMessage.Time, historicalDataMessage.Close) },
            //  historicalDataEndMessage => { visitor.RequestHistoricalDataExecuted = true;}
            //  historicalDataMessage => { macd.AddDataPoint(historicalDataMessage.Time, historicalDataMessage.Close});

            // await Task.Run(() => { while (!visitor.RequestHistoricalDataExecuted) { } });

            // DataPoint lastMacdCrossUp = macd.LastCrossUp();
            // instrument.LastSignalTime = lastMacdCrossUp.Time;

            //}

        }

    }
}

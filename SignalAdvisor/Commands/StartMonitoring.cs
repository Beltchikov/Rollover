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

            TriggerAlert(visitor);

            //foreach (var instrument in visitor.Instruments)
            //{

            //Macd macd = new Macd(12,26,9, 235.318, 235.233, 0.125);

            //await RequestHistoricalDataAndSubscribe.RunAsync(
            //  instrument,
            //  historicalDataMessage => { macd.AddDataPoint(historicalDataMessage.Time, historicalDataMessage.Close) },
            //  historicalDataEndMessage => { visitor.RequestHistoricalDataExecuted = true;}
            //  historicalDataMessage => {
            //      macd.AddDataPoint(historicalDataMessage.Time, historicalDataMessage.Close);
            //      if(macd.NewBar() && macdCrossUp())  TriggerAlert(visitor);
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

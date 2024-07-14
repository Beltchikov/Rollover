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

            //Macd macd = new Macd();

            //await RequestHistoricalDataAndSubscribe.RunAsync(
            //  visitor,
            //  historicalDataMessage => { macd.AddDataPoint(historicalDataMessage.Close) },
            //  historicalDataEndMessage => { visitor.RequestHistoricalDataExecuted = true;}
            //  historicalDataMessage => { macd.AddDataPoint(historicalDataMessage.Close});

            // await Task.Run(() => { while (!visitor.RequestHistoricalDataExecuted) { } });


        }

    }
}

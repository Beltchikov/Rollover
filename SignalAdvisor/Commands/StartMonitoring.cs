using System.Media;

namespace SignalAdvisor.Commands
{
    public class StartMonitoring
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            await RequestHistoricalData.RunAsync(visitor); 
            await Task.Run(() => { while (!visitor.RequestHistoricalDataExecuted) { } });

            UpdateItems.Run(visitor);

            _ = Task.Run(() =>
            {
                SoundPlayer player = new(Properties.Resources.Alarm01_wav);
                while (!visitor.AlertDeactivated) { player.PlaySync(); }
            });
        }

    }
}

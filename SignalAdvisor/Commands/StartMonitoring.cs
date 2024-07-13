using System.Windows;

namespace SignalAdvisor.Commands
{
    public class StartMonitoring
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {

            await Task.Run(()=> { MessageBox.Show("StartMonitoring"); });
        }

    }
}

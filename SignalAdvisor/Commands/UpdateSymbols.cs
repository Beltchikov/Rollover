using System.Windows;

namespace SignalAdvisor.Commands
{
    class UpdateSymbols
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
           await Task.Run(() => {  });
           MessageBox.Show("UpdateSymbols");
        }
    }
}

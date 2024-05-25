using System.Windows;

namespace SignalAdvisor.Commands
{
    class UpdateSymbols
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
           await Task.Run(() => {  });
           

            if(string.IsNullOrEmpty(visitor.Symbols))
            {
                MessageBox.Show("Please input the symbols.");
                return;
            }
            var symbolsSplitted = visitor.Symbols.Split(Environment.NewLine);

        }
    }
}

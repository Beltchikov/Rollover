using SignalAdvisor.Model;
using System.Windows;

namespace SignalAdvisor.Commands
{
    class UpdateSymbols
    {
        public static void Run(IPositionsVisitor visitor)
        {
            if (string.IsNullOrEmpty(visitor.Symbols))
            {
                MessageBox.Show("Please input the symbols.");
                return;
            }
            var symbolsSplitted = visitor.Symbols.Split(Environment.NewLine) ?? throw new Exception("symbolsSplitted is null");
            var instrumentsToAdd = symbolsSplitted.Select(s => new Instrument() { Symbol = s }).ToList() ?? throw new Exception("instrumentsToAdd is null");

            visitor.Instruments.Clear();    
            foreach (var instruments in instrumentsToAdd)
            {
                visitor.Instruments.Add(instruments);
            }
        }
    }
}

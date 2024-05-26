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
            
            var instrumentsTextSplitted = visitor.Symbols.Split(Environment.NewLine) ?? throw new Exception("instrumentsTextSplitted is null");
            List<Instrument> instrumentsToAdd = new List<Instrument>();
            foreach(var instrumentText in instrumentsTextSplitted)
            { 
                if(string.IsNullOrEmpty(instrumentText)) continue;
                var instrument = Instrument.FromTabbedLine(instrumentText);
                instrumentsToAdd.Add(instrument);   
            }

            visitor.Instruments.Clear();
            foreach (var instruments in instrumentsToAdd)
            {
                visitor.Instruments.Add(instruments);
            }
        }
    }
}

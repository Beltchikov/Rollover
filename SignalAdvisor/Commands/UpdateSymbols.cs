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

            if (visitor.Instruments.Any())
            {
                foreach (var instrument in instrumentsToAdd)
                {
                    // TODO
                    //visitor.IbHost.CancelMktData(instrument.RequestIdMktData);
                }
            }

            visitor.Instruments.Clear();
            foreach (var instrument in instrumentsToAdd)
            {
                // TODO
                //instrument.RequestIdMktData = requestId;

                //var requestId = visitor.IbHost.RequestMktData(
                //      contract,
                //      "",
                //      false,
                //      false,
                //      null,
                //      visitor.TickPriceCallback,
                //      s => { },
                //      (r, c, m1, m2, ex) => { });

                

                visitor.Instruments.Add(instrument);

                

            }
        }
    }
}

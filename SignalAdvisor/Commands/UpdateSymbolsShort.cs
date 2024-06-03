using IBApi;
using SignalAdvisor.Model;
using System.Windows;

namespace SignalAdvisor.Commands
{
    class UpdateSymbolsShort
    {
        public static void Run(IPositionsVisitor visitor)
        {
            if (string.IsNullOrEmpty(visitor.SymbolsShort))
            {
                MessageBox.Show("Please input the symbols.");
                return;
            }

            var instrumentsTextSplitted = visitor.SymbolsShort.Split(Environment.NewLine) ?? throw new Exception("instrumentsTextSplitted is null");
            List<Instrument> instrumentsToAdd = [];
            foreach (var instrumentText in instrumentsTextSplitted)
            {
                if (string.IsNullOrEmpty(instrumentText)) continue;
                var instrument = Instrument.FromTabbedLine(instrumentText);
                instrumentsToAdd.Add(instrument);
            }

            if (visitor.InstrumentsShort.Any())
            {
                foreach (var instrument in instrumentsToAdd)
                {
                    visitor.IbHost.CancelMktData(instrument.RequestIdMktData);
                }
            }

            visitor.InstrumentsShort.Clear();
            foreach (var instrument in instrumentsToAdd)
            {
                Contract contract = new Contract()
                {
                    ConId = instrument.ConId,
                    Symbol = instrument.Symbol,
                    SecType = App.SEC_TYPE_STK,
                    Currency = instrument.Currency,
                    Exchange = instrument.Exchange
                };

                var requestId = visitor.IbHost.RequestMktData(
                    contract,
                    "",
                    false,
                    false,
                    null,
                    visitor.TickPriceCallback,
                    s => { },
                    (r, c, m1, m2, ex) => { });

                instrument.RequestIdMktData = requestId;
                visitor.InstrumentsShort.Add(instrument);

                // Historical data
            }
        }
    }
}

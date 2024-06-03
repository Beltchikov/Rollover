using IBSampleApp.messages;
using SignalAdvisor.Model;
using System.Collections.ObjectModel;
using Ta;

namespace SignalAdvisor.Commands
{
    public interface IPositionsVisitor : ITwsVisitor
    {
        ObservableCollection<PositionMessage> Positions { get; }
        ObservableCollection<KeyValuePair<string, List<Bar>>> Bars { get; }
        ObservableCollection<KeyValuePair<string, List<Dictionary<string, int>>>> Signals { get; }
        ObservableCollection<Instrument> Instruments { get; }
        ObservableCollection<Instrument> InstrumentsShort { get; }
        void OnPropertyChanged(string propertyName);
        void AddBar(IBApi.Contract contract, HistoricalDataMessage historicalDataMessage);
        void TickPriceCallback(TickPriceMessage message);

        bool RequestPositionsExecuted { get; set; }
        bool RequestHistoricalDataExecuted { get; set; }
        string Symbols { get; set; }
        string SymbolsShort { get; set; }
        public Instrument InstrumentToTrade { get; set; }
        int OrdersSent { get; set; }
    }
}

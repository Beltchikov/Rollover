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
        ObservableCollection<KeyValuePair<string, List<int>>> Signals { get; }
        ObservableCollection<Instrument> Instruments { get;}

        void OnPropertyChanged(string propertyName);
        void AddBar(IBApi.Contract contract, HistoricalDataMessage historicalDataMessage);
        bool RequestPositionsExecuted { get; set; }
        bool RequestHistoricalDataExecuted { get; set; }
        string Symbols { get; set; }
    }
}

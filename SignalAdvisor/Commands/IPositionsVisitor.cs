using IbClient.messages;
using System.Collections.ObjectModel;
using Ta;

namespace SignalAdvisor.Commands
{
    public interface IPositionsVisitor : ITwsVisitor
    {
        ObservableCollection<PositionMessage> Positions { get; }
        ObservableCollection<KeyValuePair<string, List<Bar>>> Bars { get; }

        void OnPropertyChanged(string propertyName);
        void AddBar(IBApi.Contract contract, HistoricalDataMessage historicalDataMessage);
        bool RequestPositionsExecuted { get; set; }

    }
}

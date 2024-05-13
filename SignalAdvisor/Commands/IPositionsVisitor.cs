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

        bool RequestPositionsExecuted { get; set; }

    }
}

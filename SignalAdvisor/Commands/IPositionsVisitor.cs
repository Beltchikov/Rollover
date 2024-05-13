using IbClient.messages;
using System.Collections.ObjectModel;

namespace SignalAdvisor.Commands
{
    public interface IPositionsVisitor : ITwsVisitor
    {
        ObservableCollection<PositionMessage> Positions { get; }

        void OnPropertyChanged(string propertyName);

        bool RequestPositionsExecuted { get; set; }

    }
}

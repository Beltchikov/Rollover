using System.Collections.ObjectModel;

namespace SignalAdvisor.Commands
{
    public interface IPositionsVisitor : ITwsVisitor
    {
        ObservableCollection<string> Positions { get; }
    }
}

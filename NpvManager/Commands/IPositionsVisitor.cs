using IBSampleApp.messages;
using System.Collections.ObjectModel;

namespace NpvManager.Commands
{
    public interface IPositionsVisitor : ITwsVisitor
    {
        ObservableCollection<PositionMessage> Positions { get; }
       
    }
}

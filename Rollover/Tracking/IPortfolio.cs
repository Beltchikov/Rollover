using IBSampleApp.messages;

namespace Rollover.Tracking
{
    public interface IPortfolio
    {
        void Add(PositionMessage positionMessage);
    }
}
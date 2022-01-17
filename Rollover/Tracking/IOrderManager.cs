namespace Rollover.Tracking
{
    public interface IOrderManager
    {
        void RolloverIfNextStrike(ITrackedSymbols trackedSymbols);
    }
}
namespace Rollover.Tracking
{
    public interface IQueryParametersConverter
    {
        ITrackedSymbol TrackedSymbolForReqSecDefOptParams(ITrackedSymbol trackedSymbol);
    }
}
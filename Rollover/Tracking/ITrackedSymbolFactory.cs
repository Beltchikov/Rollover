namespace Rollover.Tracking
{
    public interface ITrackedSymbolFactory
    {
        TrackedSymbol Create(string symbol);
    }
}
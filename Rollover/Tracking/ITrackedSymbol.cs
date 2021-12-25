namespace Rollover.Tracking
{
    public interface ITrackedSymbol
    {
        string Symbol { get; set; }
        int NextStrike { get; set; }
        int OverNextStrike { get; set; }

        string ToString();
    }
}
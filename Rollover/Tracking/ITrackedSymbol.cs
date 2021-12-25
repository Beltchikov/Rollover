namespace Rollover.Tracking
{
    public interface ITrackedSymbol
    {
        string Name { get; set; }
        int NextStrike { get; set; }
        int OverNextStrike { get; set; }

        string ToString();
    }
}
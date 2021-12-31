namespace Rollover.Tracking
{
    public interface ITrackedSymbol
    {
        int ConId { get; set; }
        string Currency { get; set; }
        string Exchange { get; set; }
        string LocalSymbol { get; set; }
        int NextStrike { get; set; }
        int NextButOneStrike { get; set; }
        int ReqIdContractDetails { get; set; }
        int ReqIdSecDefOptParams { get; set; }
        string SecType { get; set; }
        double Strike { get; set; }
        string Symbol { get; set; }

        string ToString();
    }
}
namespace Rollover.Tracking
{
    public interface ITrackedSymbol
    {
        int ConId { get; set; }
        string Currency { get; set; }
        string Exchange { get; set; }
        string LocalSymbol { get; set; }
        double NextStrike { get; set; }
        double NextButOneStrike { get; set; }
        public double PreviousStrike { get; set; }
        public double PreviousButOneStrike { get; set; }
        int ReqIdContractDetails { get; set; }
        int ReqIdSecDefOptParams { get; set; }
        string SecType { get; set; }
        double Strike { get; set; }
        string Symbol { get; set; }

        string ToString();
    }
}
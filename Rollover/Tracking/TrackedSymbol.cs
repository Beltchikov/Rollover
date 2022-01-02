namespace Rollover.Tracking
{
    public class TrackedSymbol : ITrackedSymbol
    {
        public string Symbol { get; set; }
        public int ReqIdContractDetails { get; set; }
        public int ConId { get; set; }
        public string SecType { get; set; }
        public string Currency { get; set; }
        public string Exchange { get; set; }
        public double Strike { get; set; }
        public double NextStrike { get; set; }
        public double NextButOneStrike { get; set; }
        public double PreviousStrike { get; set; }
        public double PreviousButOneStrike { get; set; }
        public string LocalSymbol { get; set; }
        public int ReqIdSecDefOptParams { get; set; }

        public override string ToString()
        {
            return $"{Symbol} {NextStrike} {NextButOneStrike}";
        }
    }
}

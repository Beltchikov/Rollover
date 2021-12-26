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

        // Three last figures are behind the comma figures
        public int NextStrike { get; set; }

        // Three last figures are behind the comma figures
        public int OverNextStrike { get; set; }
        public string LocalSymbol { get; set; }

        public override string ToString()
        {
            return $"{Symbol} {NextStrike} {OverNextStrike}";
        }
    }
}

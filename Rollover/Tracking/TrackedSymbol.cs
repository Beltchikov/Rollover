namespace Rollover.Tracking
{
    public class TrackedSymbol
    {
        public string UnderlyingSymbol { get; set; }
        public int UnderlyingConId { get; set; }
        public string UnderlyingSecType { get; set; }
        public string UnderlyingCurrency { get; set; }
        public string UnderlyingExchange { get; set; }
        public double Strike { get; set; }
        public double NextStrike { get; set; }
        public double NextButOneStrike { get; set; }
        public double PreviousStrike { get; set; }
        public double PreviousButOneStrike { get; set; }
        public string UnderlyingLocalSymbol { get; set; }
        public int SellConId { get; internal set; }
        public int BuyConId { get; internal set; }

        public override string ToString()
        {
            return $"{UnderlyingLocalSymbol} {NextStrike} {NextButOneStrike}";
        }
    }
}

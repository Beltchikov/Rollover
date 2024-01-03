namespace IbClient.Types
{
    public class PriceOrError
    {
        public PriceOrError(double? price, TickType? tickType, MarketDataType? marketDataType, string error)
        {
            Price = price;
            TickType = tickType;
            MarketDataType = marketDataType;
            Error = error;
        }

        public double? Price { get; set; }
        public TickType? TickType { get; set; }
        public MarketDataType? MarketDataType { get; set; }
        public string Error { get; set; }
    }
}

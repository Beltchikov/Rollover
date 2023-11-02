namespace StockAnalyzer.DataProviders.Types
{
    public record Price (double? Value, int MarketDataType, int TickType)
    {
    }
}

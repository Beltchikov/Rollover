namespace EventTrader.EconomicData.Strategies
{
    public interface IEconomicDataProvider
    {
        (double?, double?, double?) GetData(
            string url,
            string xPathActual,
            string xPathExpected,
            string xPathPrevious,
            string nullPlaceholder);
    }
}

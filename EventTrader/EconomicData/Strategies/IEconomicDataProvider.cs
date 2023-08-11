namespace EventTrader.EconomicData.Strategies
{
    public interface IEconomicDataProvider
    {
        (double?, double?, double?) GetData();
    }
}

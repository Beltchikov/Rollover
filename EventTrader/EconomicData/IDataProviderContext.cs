namespace EventTrader.EconomicData
{
    public interface IDataProviderContext
    {
        (double?, double?, double?) GetData();
        void SetStrategy(string dataType);
    }
}
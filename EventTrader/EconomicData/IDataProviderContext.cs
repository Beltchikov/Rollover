namespace EventTrader.EconomicData
{
    public interface IDataProviderContext
    {
        void GetData();
        void SetStrategy(string dataType);
    }
}
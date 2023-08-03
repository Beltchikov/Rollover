namespace EventTrader.EconomicData
{
    public interface IEconomicDataTrader
    {
        void StartSession(EconomicDataTrade trade);
    }
}
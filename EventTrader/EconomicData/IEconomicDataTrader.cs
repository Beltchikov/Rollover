using System.Threading.Tasks;

namespace EventTrader.EconomicData
{
    public interface IEconomicDataTrader
    {
        Task StartSessionAsync(EconomicDataTrade trade);
    }
}
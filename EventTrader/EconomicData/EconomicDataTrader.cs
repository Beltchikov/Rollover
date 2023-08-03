using System.Threading.Tasks;
using System.Windows;

namespace EventTrader.EconomicData
{
    public class EconomicDataTrader : IEconomicDataTrader
    {
        public async Task StartSessionAsync(EconomicDataTrade trade)
        {
            await Task.Run(() => MessageBox.Show("Not implemented yet"));
        }
    }
}

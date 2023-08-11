using System;

namespace EventTrader.EconomicData.Strategies
{
    public class FakedDataProvider : IEconomicDataProvider
    {
        public (double, double, double) GetData()
        {
            // TODO
            return (10.5, 10.6, 10.2);
        }
    }
}

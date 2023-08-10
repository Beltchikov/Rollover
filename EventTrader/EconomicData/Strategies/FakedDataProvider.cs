using System;

namespace EventTrader.EconomicData.Strategies
{
    public class FakedDataProvider : IEconomicDataProvider
    {
        public (double, double, double) GetData(string url, object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}

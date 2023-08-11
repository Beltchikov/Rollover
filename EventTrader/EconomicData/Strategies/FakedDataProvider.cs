using System;

namespace EventTrader.EconomicData.Strategies
{
    public class FakedDataProvider : IEconomicDataProvider
    {
        public (double?, double?, double?) GetData()
        {
            // TODO
            return (null, 10.6, 10.2);
        }
    }
}

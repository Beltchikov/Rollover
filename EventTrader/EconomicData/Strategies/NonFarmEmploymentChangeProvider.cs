using EventTrader.WebScraping;
using System;

namespace EventTrader.EconomicData.Strategies
{
    public class NonFarmEmploymentChangeProvider : IEconomicDataProvider
    {
        IBrowserWrapper _browserWrapper;

        public NonFarmEmploymentChangeProvider(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
        }

        public (double?, double?, double?) GetData()
        {
            throw new NotImplementedException();
        }
    }
}

using EventTrader.WebScraping;
using System;

namespace EventTrader.EconomicData.Strategies
{
    public class NonFarmEmploymentChangeProvider : IEconomicDataProvider
    {
        IBrowserWrapper _browserWrapper;
        const string URL = "https://www.investing.com/economic-calendar/";

        public NonFarmEmploymentChangeProvider(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
        }

        public (double?, double?, double?) GetData()
        {
            if(! _browserWrapper.Navigate(URL))
            {
                throw new ApplicationException($"Can not navigate to {URL}");
            }

            var htmlSource = _browserWrapper.Text;

            throw new NotImplementedException();
        }
    }
}

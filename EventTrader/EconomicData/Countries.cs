using System.Collections.Generic;

namespace EventTrader.EconomicData
{
    public class Countries
    {
        public Countries()
        {
            All = new List<Country>()
            {
                new Country("US"),
                new Country("EU"),
                new Country("JP"),
                new Country("GB"),
                new Country("CA"),
                new Country("AU"),
                new Country("CH"),
                new Country("NZ")
            };
        }

        public List<Country> All { get; private set; }
    }
}

using System.Collections.Generic;

namespace Dsmn.EconomicData
{
    public static class Countries
    {
        static Countries()
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

        public static List<Country> All { get; private set; }
    }
}

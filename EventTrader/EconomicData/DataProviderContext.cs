using EventTrader.EconomicData.Strategies;
using EventTrader.WebScraping;
using System;

namespace EventTrader.EconomicData
{
    public class DataProviderContext : IDataProviderContext
    {
        IEconomicDataProvider _provider = null!;
        IBrowserWrapper _browserWrapper;

        public DataProviderContext(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
        }

        (double?, double?, double?) IDataProviderContext.GetData()
        {
            return _provider.GetData();
        }

        public void SetStrategy(string dataType)
        {
            DataTypeEnum dataTypeAsEnum = (DataTypeEnum)Enum.Parse(typeof(DataTypeEnum), dataType);
            switch (dataTypeAsEnum)
            {
                case DataTypeEnum.NonFarmEmploymentChange:
                    {
                        //_provider = new FakedDataProvider();
                        _provider = new NonFarmEmploymentChangeProvider(_browserWrapper);
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }

        }
    }
}

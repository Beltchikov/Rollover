using EventTrader.EconomicData.Strategies;
using System;

namespace EventTrader.EconomicData
{
    public class DataProviderContext : IDataProviderContext
    {
        IEconomicDataProvider _provider = null!;

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
                        _provider = new FakedDataProvider(); // TODO
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

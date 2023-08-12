using EventTrader.EconomicData.Strategies;
using EventTrader.WebScraping;
using System;
using System.Linq;

namespace EventTrader.EconomicData
{
    public class DataProviderContext : IDataProviderContext
    {
        DataTypeEnum _dataTypeAsEnum;
        IEconomicDataProvider _provider = null!;
        IBrowserWrapper _browserWrapper;

        public DataProviderContext(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
        }

        public void SetStrategy(string dataType)
        {
            _dataTypeAsEnum = (DataTypeEnum)Enum.Parse(typeof(DataTypeEnum), dataType);
            switch (_dataTypeAsEnum)
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

        (double?, double?, double?) IDataProviderContext.GetData()
        {
            // TODO
            return _provider.GetData("todo", "todo", "todo", "todo", "todo");
        }
    }
}

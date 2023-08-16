using EventTrader.EconomicData.Strategies;
using EventTrader.WebScraping;
using System;

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

        public (double?, double?, double?) GetData(
            string url,
            string xPathActual,
            string xPathExpected,
            string xPathPrevious,
            string nullPlaceholder)
        {
            return _provider.GetData(url, xPathActual, xPathExpected, xPathPrevious, nullPlaceholder);
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
    }
}

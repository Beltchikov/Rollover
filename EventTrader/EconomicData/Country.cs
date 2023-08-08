using System;
using System.Collections.Generic;

namespace EventTrader.EconomicData
{
    public class Country
    {
        public Country(string symbol)
        {
            Symbol = symbol;

            switch (symbol)
            {
                case "US":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdUs),
                        new Data(DataTypeEnum.AverageHourlyEarnings),
                        new Data(DataTypeEnum.CoreCpiMmUs),
                        new Data(DataTypeEnum.InitialJoblessClaims),
                        new Data(DataTypeEnum.AdpNonFarmEmploymentChange),
                        new Data(DataTypeEnum.NonFarmEmploymentChange)
                    };
                    break;
                case "EU":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdEu),
                        new Data(DataTypeEnum.GermanPreliminaryCpiMm),
                        new Data(DataTypeEnum.CpiFlashEstimateYy),
                        new Data(DataTypeEnum.UnemploymentRateEu)
                    };
                    break;
                case "JP":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdJp),
                        new Data(DataTypeEnum.TokioCoreCpiYy),
                        new Data(DataTypeEnum.NationalCoreCpiYy),
                        new Data(DataTypeEnum.UnemploymentRateJp)
                    };
                    break;
                case "GB":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdGb),
                        new Data(DataTypeEnum.CpiYyGb),
                        new Data(DataTypeEnum.ClaimantCountChange)
                };
                    break;
                default:
                    throw new NotImplementedException();
            }

            // todo fill DataList
        }

        public string Symbol { get; private set; }
        public List<Data> DataList { get; private set; }
    }
}
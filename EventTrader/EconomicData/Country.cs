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
                        new Data(DataTypeEnum.NationalCoreCpiYyJp),
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

                case "CA":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdCa),
                        new Data(DataTypeEnum.CpiMmCa),
                        new Data(DataTypeEnum.MedianCpiYyCa),
                        new Data(DataTypeEnum.TrimmedCpiYyCa),
                        new Data(DataTypeEnum.UnemploymentRateCa),
                    };
                    break;
                case "AU":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdAu),
                        new Data(DataTypeEnum.CommodityPricesYy),
                        new Data(DataTypeEnum.MiInflationGaugeMm),
                        new Data(DataTypeEnum.CpiYyAu),
                        new Data(DataTypeEnum.CpiQqAu),
                        new Data(DataTypeEnum.TrimmedMeanCpiQqAu),
                        new Data(DataTypeEnum.UnemploymentRateAu)
                    };
                    break;
                case "CH":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdCh),
                        new Data(DataTypeEnum.CpiMmCh),
                        new Data(DataTypeEnum.UnemploymentRateCh)
                    };
                    break;
                case "NZ":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdNz),
                        new Data(DataTypeEnum.NationalCoreCpiYyNz),
                        new Data(DataTypeEnum.CpiQqNz),
                        new Data(DataTypeEnum.GdtPriceIndex),
                        new Data(DataTypeEnum.UnemploymentRateNz)
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
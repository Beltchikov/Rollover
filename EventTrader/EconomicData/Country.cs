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
                        new Data(DataTypeEnum.IrdUs, "IRD"),
                        new Data(DataTypeEnum.AverageHourlyEarnings, "Average Hourly Earnings"),
                        new Data(DataTypeEnum.CoreCpiMmUs, "Core CPI m/m"),
                        new Data(DataTypeEnum.InitialJoblessClaims, "Initial Jobless Claims"),
                        new Data(DataTypeEnum.AdpNonFarmEmploymentChange, "ADP Non-Farm Employment Change"),
                        new Data(DataTypeEnum.NonFarmEmploymentChange, "Non-Farm Employment Change")
                    };
                    break;
                case "EU":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdEu, "IRD"),
                        new Data(DataTypeEnum.GermanPreliminaryCpiMm, "German Preliminary CPI m/m"),
                        new Data(DataTypeEnum.CpiFlashEstimateYy, "CPI Flash Estimate y/y"),
                        new Data(DataTypeEnum.UnemploymentRateEu, "Unemployment Rate")
                    };
                    break;
                case "JP":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdJp, "IRD"),
                        new Data(DataTypeEnum.TokioCoreCpiYy, "Tokio Core CPI y/y"),
                        new Data(DataTypeEnum.NationalCoreCpiYyJp, "National Core CPI y/y"),
                        new Data(DataTypeEnum.UnemploymentRateJp, "Unemployment Rate")
                    };
                    break;
                case "GB":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdGb, "IRD"),
                        new Data(DataTypeEnum.CpiYyGb, "CPI y/y"),
                        new Data(DataTypeEnum.ClaimantCountChange, "Claimant Count Change")
                    };
                    break;

                case "CA":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdCa, "IRD"),
                        new Data(DataTypeEnum.CpiMmCa, "CPI m/m"),
                        new Data(DataTypeEnum.MedianCpiYyCa, "Median CPI y/y"),
                        new Data(DataTypeEnum.TrimmedCpiYyCa, "Trimmed CPI y/y"),
                        new Data(DataTypeEnum.UnemploymentRateCa, "Unemployment Rate"),
                    };
                    break;
                case "AU":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdAu, "IRD"),
                        new Data(DataTypeEnum.CommodityPricesYy, "Commodity Prices y/y"),
                        new Data(DataTypeEnum.MiInflationGaugeMm, "MI Inflation Gauge m/m"),
                        new Data(DataTypeEnum.CpiYyAu, "CPI y/y"),
                        new Data(DataTypeEnum.CpiQqAu, "CPI q/q"),
                        new Data(DataTypeEnum.TrimmedMeanCpiQqAu, "Trimmed Mean CPI q/q"),
                        new Data(DataTypeEnum.UnemploymentRateAu, "Unemployment Rate")
                    };
                    break;
                case "CH":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdCh, "IRD"),
                        new Data(DataTypeEnum.CpiMmCh, "CPI m/m"),
                        new Data(DataTypeEnum.UnemploymentRateCh, "Unemployment Rate")
                    };
                    break;
                case "NZ":
                    DataList = new List<Data>
                    {
                        new Data(DataTypeEnum.IrdNz, "IRD"),
                        new Data(DataTypeEnum.NationalCoreCpiYyNz, "National Core CPI y/y"),
                        new Data(DataTypeEnum.CpiQqNz, "CPI q/q"),
                        new Data(DataTypeEnum.GdtPriceIndex, "GDT Price Index"),
                        new Data(DataTypeEnum.UnemploymentRateNz, "Unemployment Rate")
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
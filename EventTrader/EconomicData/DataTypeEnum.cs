namespace EventTrader.EconomicData
{
    public enum DataTypeEnum
    {
        // US IRD, Inflation
        IrdUs,
        AverageHourlyEarnings,
        CoreCpiMmUs,
        // US Employment
        InitialJoblessClaims,
        AdpNonFarmEmploymentChange,
        NonFarmEmploymentChange,
        // EU IRD, Inflation
        IrdEu,
        GermanPreliminaryCpiMm,
        CpiFlashEstimateYy,
        // EU Employment
        UnemploymentRateEu,
        // JP IRD, Inflation
        IrdJp,
        TokioCoreCpiYy,
        NationalCoreCpiYyJp,
        // JP Employment
        UnemploymentRateJp,
        // GB IRD, Inflation
        IrdGb,
        CpiYyGb,
        // GB Employment
        ClaimantCountChange,

        // CA IRD, Inflation
        IrdCa,
        CpiMmCa,
        MedianCpiYyCa,
        TrimmedCpiYyCa,
        // CA Employment
        UnemploymentRateCa,

        // AU IRD, Inflation
        IrdAu,
        CommodityPricesYy,
        MiInflationGaugeMm,
        CpiYyAu,
        CpiQqAu,
        TrimmedMeanCpiQqAu,
        // AU Employment
        UnemploymentRateAu,

        // CH IRD, Inflation
        IrdCh,
        CpiMmCh,
        // CH Employment
        UnemploymentRateCh,

        // NZ IRD, Inflation
        IrdNz,
        NationalCoreCpiYyNz,
        CpiQqNz,
        GdtPriceIndex,
        // NZ Employment
        UnemploymentRateNz
    }
}
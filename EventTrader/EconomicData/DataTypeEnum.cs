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
        NationalCoreCpiYy,
        // JP Employment
        UnemploymentRateJp,
        // GB IRD, Inflation
        IrdGb,
        CpiYyGb,
        // GB Employment
        ClaimantCountChange
    }
}
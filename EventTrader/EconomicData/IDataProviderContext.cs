namespace Dsmn.EconomicData
{
    public interface IDataProviderContext
    {
        (double?, double?, double?) GetData(
            string url,
            string xPathActual,
            string xPathExpected,
            string xPathPrevious,
            string nullPlaceholder);
        void SetStrategy(string dataType);
    }
}
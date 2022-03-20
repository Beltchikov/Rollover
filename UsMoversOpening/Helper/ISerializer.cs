namespace UsMoversOpening.Helper
{
    public interface ISerializer
    {
        T Deserialize<T>(string configurationAsText);
        string Serialize(object objectToSerialize);
    }
}
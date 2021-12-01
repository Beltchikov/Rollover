namespace Rollover
{
    public interface ISerializer
    {
        T Deserialize<T>(string configurationAsText);
    }
}
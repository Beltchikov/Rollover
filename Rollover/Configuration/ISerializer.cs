namespace Rollover.Configuration
{
    public interface ISerializer
    {
        T Deserialize<T>(string configurationAsText);
    }
}
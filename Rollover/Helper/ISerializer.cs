namespace Rollover.Helper
{
    public interface ISerializer
    {
        T Deserialize<T>(string configurationAsText);
    }
}
using System.Text.Json;

namespace Rollover.Configuration
{
    public class Serializer : ISerializer
    {
        public T Deserialize<T>(string configurationAsText)
        {
            return JsonSerializer.Deserialize<T>(configurationAsText);
        }
    }
}

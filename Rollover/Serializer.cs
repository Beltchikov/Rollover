using System.Text.Json;

namespace Rollover
{
    public class Serializer : ISerializer
    {
        public T Deserialize<T>(string configurationAsText)
        {
            return JsonSerializer.Deserialize<T>(configurationAsText);
        }
    }
}

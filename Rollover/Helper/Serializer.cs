﻿using System.Text.Json;

namespace Rollover.Helper
{
    public class Serializer : ISerializer
    {
        public T Deserialize<T>(string configurationAsText)
        {
            return JsonSerializer.Deserialize<T>(configurationAsText);
        }

        public string Serialize(object objectToSerialize)
        {
            return JsonSerializer.Serialize(objectToSerialize);
        }
    }
}

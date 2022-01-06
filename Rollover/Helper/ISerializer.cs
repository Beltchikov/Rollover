using Rollover.Tracking;
using System.Collections.Generic;

namespace Rollover.Helper
{
    public interface ISerializer
    {
        T Deserialize<T>(string configurationAsText);
        void Serialize(object objectToSerialize);
    }
}
using System.Collections.Generic;

namespace Rollover.Ib
{
    public class SecTypeConverter : ISecTypeConverter
    {
        private Dictionary<string, string> _secTypeMap;

        public SecTypeConverter()
        {

        }

        public string GetUnderlyingSecType(string derivativeSecType)
        {
            return _secTypeMap[derivativeSecType];
        }
    }
}
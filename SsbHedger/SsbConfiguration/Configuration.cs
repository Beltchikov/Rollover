using System.Collections.Generic;

namespace SsbHedger.SsbConfiguration
{
    public class Configuration : IConfiguration
    {
        public const string HOST = "Host";
        public const string PORT = "Port";
        public const string CLIENT_ID = "ClientId";
        public const string UNDERLYING_SYMBOL = "UnderlyingSymbol";
        public const string SESSION_START = "SessionStart";
        public const string SESSION_END = "SessionEnd";

        private Dictionary<string, object> _configuration;

        public Configuration()
        {
            _configuration = new Dictionary<string, object>()
            {
                {HOST, "localhost" },
                {PORT, 4001 },
                {CLIENT_ID, 1 },
                {UNDERLYING_SYMBOL, "SPY" },
                {SESSION_START, "15:30" },
                {SESSION_END, "22:15" },
            };
        }

        public void SetValue(string name, object value)
        {
            _configuration[name] = value;
        }

        public object GetValue(string name)
        {
            return _configuration[name];
        }
    }
}

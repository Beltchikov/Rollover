namespace SsbHedger.Configuration
{
    public record struct ConfigurationData(string Host,
        int Port,
        int ClientId,
        string UnderlyingSymbol,
        string SessionStart,
        string SessionEnd)
    {
        public static implicit operator (string host, int port, int clientId, string underlyingSymbol, string sessionStart, string sessionEnd)(ConfigurationData value)
        {
            return (value.Host, value.Port, value.ClientId, value.UnderlyingSymbol, value.SessionStart, value.SessionEnd);
        }

        public static implicit operator ConfigurationData((string host, int port, int clientId, string underlyingSymbol, string sessionStart, string sessionEnd) value)
        {
            return new ConfigurationData(value.host, value.port, value.clientId, value.underlyingSymbol, value.sessionStart, value.sessionEnd);
        }
    }
}
namespace SsbHedger.SsbConfiguration
{
    public record struct ConfigurationData(string Host,
        int Port,
        int ClientId,
        string UnderlyingSymbol,
        string SessionStart,
        string SessionEnd)
    {
        public static implicit operator (
            string host, 
            int port, 
            int clientId, 
            string underlyingSymbol, 
            string sessionStart, 
            string sessionEnd)
            (ConfigurationData value) => (
            value.Host, 
            value.Port, 
            value.ClientId, 
            value.UnderlyingSymbol, 
            value.SessionStart, 
            value.SessionEnd);

        public static implicit operator ConfigurationData((
            string host, 
            int port, 
            int clientId, 
            string underlyingSymbol, 
            string sessionStart, 
            string sessionEnd,
            double bearHedgeStrike,
            double bullHedgeStrike) value) => new ConfigurationData(
                value.host, 
                value.port, 
                value.clientId, 
                value.underlyingSymbol, 
                value.sessionStart, 
                value.sessionEnd);
    }
}
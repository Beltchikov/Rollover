namespace SsbHedger.SsbConfiguration
{
    public record struct ConfigurationData(string Host,
        int Port,
        int ClientId,
        string UnderlyingSymbol,
        string SessionStart,
        string SessionEnd,
        int Dte,
        int NumberOfStrikes)
    {
        public static implicit operator (
            string host, 
            int port, 
            int clientId, 
            string underlyingSymbol, 
            string sessionStart, 
            string sessionEnd,
            int dte,
            int numberOfStrikes)
            (ConfigurationData value) => (
            value.Host, 
            value.Port, 
            value.ClientId, 
            value.UnderlyingSymbol, 
            value.SessionStart, 
            value.SessionEnd,
            value.Dte,
            value.NumberOfStrikes);

        public static implicit operator ConfigurationData((
            string host, 
            int port, 
            int clientId, 
            string underlyingSymbol, 
            string sessionStart, 
            string sessionEnd,
            int dte,
            int numberOfStrikes) value) => new ConfigurationData(
                value.host, 
                value.port, 
                value.clientId, 
                value.underlyingSymbol, 
                value.sessionStart, 
                value.sessionEnd,
                value.dte,
                value.numberOfStrikes);
    }
}
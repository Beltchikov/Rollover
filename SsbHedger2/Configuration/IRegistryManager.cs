namespace SsbHedger.Configuration
{
    public interface IRegistryManager
    {
        public (string host, int port, int clientId) ReadConfiguration(
            string defaultHost,
            int defaultPort,
            int defaultClientId);
        void WriteConfiguration(string host, int port, int clientId);
    }
}
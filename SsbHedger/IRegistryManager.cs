namespace SsbHedger
{
    public interface IRegistryManager
    {
        public (string host, int port, int clientId) ReadConfiguration(
            string defaultHost,
            int defaultPort,
            int defaultClientId);
    }
}
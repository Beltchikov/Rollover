using System;

namespace SsbHedger
{
    public class RegistryManager : IRegistryManager
    {
        (string host, int port, int clientId) IRegistryManager.ReadConfiguration(string defaultHost, int defaultPort, int defaultClientId)
        {
            return (defaultHost, defaultPort, defaultClientId);
            //throw new NotImplementedException();
        }
    }
}
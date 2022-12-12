using System;

namespace SsbHedger
{
    public class RegistryManager : IRegistryManager
    {
        (string host, int port, int clientId) IRegistryManager.ReadConfiguration(string defaultHost, int defaultPort, int defaultClientId)
        {
            //return (defaultHost, defaultPort, defaultClientId);

            // TODO
            // RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\SsbHedger");  
            // https://www.c-sharpcorner.com/UploadFile/f9f215/windows-registry/

            throw new NotImplementedException();
        }
    }
}
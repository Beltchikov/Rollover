using Microsoft.Win32;
using SsbHedger.Abstractions;
using System;

namespace SsbHedger
{
    public class RegistryManager : IRegistryManager
    {
        private const string SOFTWARE_SSBHEDGER = @"SOFTWARE\SsbHedger";
        private const string HOST = @"Host";
        private const string PORT = @"Port";
        private const string CLIENT_ID = @"ClientId";
        private IRegistryCurrentUserAbstraction _registryCurrentUser = new RegistryCurrentUserAbstraction();

        public (string host, int port, int clientId) ReadConfiguration(
            string defaultHost,
            int defaultPort,
            int defaultClientId)
        {
            
            var subKey = _registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER);
            if(subKey == null)
            {
                return new ValueTuple<string, int, int>(defaultHost, defaultPort, defaultClientId);
            }

            var host = subKey.GetValue(HOST)?.ToString(); 
            var port = (int?)subKey.GetValue(PORT);
            var clientId = (int?)subKey.GetValue(CLIENT_ID);
            if (host == null || port == null || clientId == null)
            {
                return new ValueTuple<string, int, int>(defaultHost, defaultPort, defaultClientId);
            }

            return new ValueTuple<string, int, int>(host, port.Value, clientId.Value);

            ////////////////////////
            // RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\SsbHedger");
            // https://www.c-sharpcorner.com/UploadFile/f9f215/windows-registry/
        }
    }
}
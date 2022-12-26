using SsbHedger2.Abstractions;
using System;

namespace SsbHedger2
{
    public class RegistryManager : IRegistryManager
    {
        private const string SOFTWARE_SSBHEDGER = @"SOFTWARE\SsbHedger";
        private const string HOST = @"Host";
        private const string PORT = @"Port";
        private const string CLIENT_ID = @"ClientId";
        private IRegistryCurrentUserAbstraction _registryCurrentUser;

        public RegistryManager(IRegistryCurrentUserAbstraction registryCurrentUser)
        {
            _registryCurrentUser = registryCurrentUser;
        }

        public (string host, int port, int clientId) ReadConfiguration(
            string defaultHost,
            int defaultPort,
            int defaultClientId)
        {

            var subKey = _registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER);
            if (subKey == null)
            {
                subKey = _registryCurrentUser.CreateSubKey(SOFTWARE_SSBHEDGER);
                subKey.SetValue(HOST, defaultHost);
                subKey.SetValue(PORT, defaultPort);
                subKey.SetValue(CLIENT_ID, defaultClientId);

                return new ValueTuple<string, int, int>(defaultHost, defaultPort, defaultClientId);
            }

            object?[] configValuesFromRegistry = new object?[]
            {
                subKey.GetValue(HOST)?.ToString(),
                (int?)subKey.GetValue(PORT),
                (int?)subKey.GetValue(CLIENT_ID)
            };
            object?[] configValuesValidated = new object?[3];
            for(int i = 0; i < 3; i++)
            {
                var configValue = configValuesFromRegistry[i];
                switch (i)
                {
                    case 0:
                        configValuesValidated[i] = String.IsNullOrWhiteSpace(configValue?.ToString())
                            ? defaultHost
                            : configValue?.ToString();
                        break;
                    case 1:
                        configValuesValidated[i] = ValidPortAndClientId((int?)subKey.GetValue(PORT)) 
                            ? defaultPort
                            : (int?)subKey.GetValue(PORT);
                        break;
                    case 2:
                        configValuesValidated[i] = ValidPortAndClientId((int?)subKey.GetValue(CLIENT_ID))
                            ? defaultClientId
                            : (int?)subKey.GetValue(CLIENT_ID);
                        break;
                }
            }

            return new ValueTuple<string, int, int>(
                configValuesValidated[0]?.ToString(),
                port.Value,
                clientId.Value);

            //var host = subKey.GetValue(HOST)?.ToString();
            //var port = (int?)subKey.GetValue(PORT);
            //var clientId = (int?)subKey.GetValue(CLIENT_ID);
            //if (host == null || port == null || clientId == null)
            //{
            //    return new ValueTuple<string, int, int>(defaultHost, defaultPort, defaultClientId);
            //}

            //return new ValueTuple<string, int, int>(host, port.Value, clientId.Value);
        }

        public void WriteConfiguration(string host, int port, int clientId)
        {
            var subKey = _registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER)
                ?? _registryCurrentUser.CreateSubKey(SOFTWARE_SSBHEDGER);

            subKey.SetValue(HOST, host);
            subKey.SetValue(PORT, port);
            subKey.SetValue(CLIENT_ID, clientId);
        }

        private bool ValidPortAndClientId(int? port)
        {
            if(port == null)
            {
                return false;
            }

            return port > 0;
        }
    }
}
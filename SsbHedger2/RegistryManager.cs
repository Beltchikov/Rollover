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
            object[] configValuesValidated = new object[3];
            for(int i = 0; i < 3; i++)
            {
                var configValue = configValuesFromRegistry[i];
                switch (i)
                {
                    case 0:
                        var configValue1Typed = configValue?.ToString();
                        if(!String.IsNullOrWhiteSpace(configValue1Typed))
                        {
                            configValuesValidated[i] = configValue1Typed;
                        }
                        else
                        {
                            configValuesValidated[i] = defaultHost;
                            subKey.SetValue(HOST, defaultHost);
                        }
                        break;
                    case 1:
                        var configValue2Typed = (int?)subKey.GetValue(PORT);
                        if (configValue2Typed != null && configValue2Typed > 0)
                        {
                            configValuesValidated[i] = configValue2Typed;
                        }
                        else
                        {
                            configValuesValidated[i] = defaultPort;
                            subKey.SetValue(PORT, defaultPort);
                        }
                        break;
                    case 2:
                        var configValue3Typed = (int?)subKey.GetValue(CLIENT_ID);
                        configValuesValidated[i] = configValue3Typed != null && configValue3Typed > 0
                            ? configValue3Typed
                            : defaultClientId;
                     break;
                }
            }

            return new ValueTuple<string, int, int>(
                (string)configValuesValidated[0],
                (int)configValuesValidated[1],
                (int)configValuesValidated[2]);
        }

        public void WriteConfiguration(string host, int port, int clientId)
        {
            var subKey = _registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER)
                ?? _registryCurrentUser.CreateSubKey(SOFTWARE_SSBHEDGER);

            subKey.SetValue(HOST, host);
            subKey.SetValue(PORT, port);
            subKey.SetValue(CLIENT_ID, clientId);
        }
    }
}
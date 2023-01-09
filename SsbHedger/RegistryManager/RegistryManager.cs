using SsbHedger.Abstractions;
using SsbHedger.SsbConfiguration;
using System;

namespace SsbHedger.RegistryManager
{
    public class RegistryManager : IRegistryManager
    {
        private const string SOFTWARE_SSBHEDGER = @"SOFTWARE\SsbHedger";
        private const string HOST = @"Host";
        private const string PORT = @"Port";
        private const string CLIENT_ID = @"ClientId";
        private const string UNDERLYING_SYMBOL = @"UnderlyingSymbol";
        private const string SESSION_START = @"SessionStart";
        private const string SESSION_END= @"SessionEnd";
        private IRegistryCurrentUserAbstraction _registryCurrentUser;
        
        public RegistryManager(IRegistryCurrentUserAbstraction registryCurrentUser)
        {
            _registryCurrentUser = registryCurrentUser;
        }

        public ConfigurationData ReadConfiguration(ConfigurationData defaultConfigurationData)
        {
            var subKey = _registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER);
            if (subKey == null)
            {
                subKey = _registryCurrentUser.CreateSubKey(SOFTWARE_SSBHEDGER);
                subKey.SetValue(HOST, defaultConfigurationData.Host);
                subKey.SetValue(PORT, defaultConfigurationData.Port);
                subKey.SetValue(CLIENT_ID, defaultConfigurationData.ClientId);
                subKey.SetValue(UNDERLYING_SYMBOL, defaultConfigurationData.UnderlyingSymbol);
                subKey.SetValue(SESSION_START, defaultConfigurationData.SessionStart);
                subKey.SetValue(SESSION_END, defaultConfigurationData.SessionEnd);

                return new ValueTuple<string, int, int, string, string, string>(
                    defaultConfigurationData.Host,
                    defaultConfigurationData.Port,
                    defaultConfigurationData.ClientId,
                    defaultConfigurationData.UnderlyingSymbol,
                    defaultConfigurationData.SessionStart,
                    defaultConfigurationData.SessionEnd);
            }

            var defaultOrFromRegistryConfigData = new ConfigurationData();

            var hostFromRegistry = subKey?.GetValue(HOST)?.ToString();
            if(!string.IsNullOrWhiteSpace(hostFromRegistry))
            {
                defaultOrFromRegistryConfigData.Host = hostFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.Host = defaultConfigurationData.Host;
                subKey?.SetValue(HOST, defaultConfigurationData.Host);
            }

            var portFromRegistry = (int?)subKey?.GetValue(PORT);
            if (portFromRegistry != null && portFromRegistry > 0)
            {
                defaultOrFromRegistryConfigData.Port = (int)portFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.Port = defaultConfigurationData.Port;
                subKey?.SetValue(PORT, defaultConfigurationData.Port);
            }

            var clientIdFromRegistry = (int?)subKey?.GetValue(CLIENT_ID);
            if (clientIdFromRegistry != null && clientIdFromRegistry > 0)
            {
                defaultOrFromRegistryConfigData.ClientId = (int)clientIdFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.ClientId = defaultConfigurationData.ClientId;
                subKey?.SetValue(CLIENT_ID, defaultConfigurationData.ClientId);
            }

            var underlyingSymbolFromRegistry = subKey?.GetValue(UNDERLYING_SYMBOL)?.ToString();
            if (!string.IsNullOrWhiteSpace(underlyingSymbolFromRegistry))
            {
                defaultOrFromRegistryConfigData.UnderlyingSymbol = underlyingSymbolFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.UnderlyingSymbol = defaultConfigurationData.UnderlyingSymbol;
                subKey?.SetValue(UNDERLYING_SYMBOL, defaultConfigurationData.UnderlyingSymbol);
            }

            var sessionStartFromRegistry = subKey?.GetValue(SESSION_START)?.ToString();
            if (!string.IsNullOrWhiteSpace(sessionStartFromRegistry))
            {
                defaultOrFromRegistryConfigData.SessionStart = sessionStartFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.SessionStart = defaultConfigurationData.SessionStart;
                subKey?.SetValue(SESSION_START, defaultConfigurationData.SessionStart);
            }

            var sessionEndFromRegistry = subKey?.GetValue(SESSION_END)?.ToString();
            if (!string.IsNullOrWhiteSpace(sessionEndFromRegistry))
            {
                defaultOrFromRegistryConfigData.SessionEnd = sessionEndFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.SessionEnd = defaultConfigurationData.SessionEnd;
                subKey?.SetValue(SESSION_END, defaultConfigurationData.SessionEnd);
            }

            return new ValueTuple<string, int, int, string, string, string>(
                defaultOrFromRegistryConfigData.Host,
                defaultOrFromRegistryConfigData.Port,
                defaultOrFromRegistryConfigData.ClientId,
                defaultOrFromRegistryConfigData.UnderlyingSymbol,
                defaultOrFromRegistryConfigData.SessionStart,
                defaultOrFromRegistryConfigData.SessionEnd);
        }

        public void WriteConfiguration(ConfigurationData defaultConfigurationData)
        {
            var subKey = _registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER)
                ?? _registryCurrentUser.CreateSubKey(SOFTWARE_SSBHEDGER);

            subKey.SetValue(HOST, defaultConfigurationData.Host);
            subKey.SetValue(PORT, defaultConfigurationData.Port);
            subKey.SetValue(CLIENT_ID, defaultConfigurationData.ClientId);
            subKey.SetValue(UNDERLYING_SYMBOL, defaultConfigurationData.UnderlyingSymbol);
            subKey.SetValue(SESSION_START, defaultConfigurationData.SessionStart);
            subKey.SetValue(SESSION_END, defaultConfigurationData.SessionEnd);
        }
    }
}
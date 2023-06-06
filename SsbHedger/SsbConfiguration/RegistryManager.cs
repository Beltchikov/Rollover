using SsbHedger.Abstractions;
using System;
using System.Globalization;

namespace SsbHedger.SsbConfiguration
{
    public class RegistryManager : IRegistryManager
    {
        private const string SOFTWARE_SSBHEDGER = @"SOFTWARE\SsbHedger";
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
                subKey.SetValue(Configuration.HOST, defaultConfigurationData.Host);
                subKey.SetValue(Configuration.PORT, defaultConfigurationData.Port);
                subKey.SetValue(Configuration.CLIENT_ID, defaultConfigurationData.ClientId);
                subKey.SetValue(Configuration.UNDERLYING_SYMBOL, defaultConfigurationData.UnderlyingSymbol);
                subKey.SetValue(Configuration.SESSION_START, defaultConfigurationData.SessionStart);
                subKey.SetValue(Configuration.SESSION_END, defaultConfigurationData.SessionEnd);
                subKey.SetValue(Configuration.DTE, defaultConfigurationData.Dte);
                subKey.SetValue(Configuration.NUMBER_OF_STRIKES, defaultConfigurationData.NumberOfStrikes);

                return new ConfigurationData(
                    defaultConfigurationData.Host,
                    defaultConfigurationData.Port,
                    defaultConfigurationData.ClientId,
                    defaultConfigurationData.UnderlyingSymbol,
                    defaultConfigurationData.SessionStart,
                    defaultConfigurationData.SessionEnd,
                    defaultConfigurationData.Dte,
                    defaultConfigurationData.NumberOfStrikes);
            }

            var defaultOrFromRegistryConfigData = new ConfigurationData();

            var hostFromRegistry = subKey?.GetValue(Configuration.HOST)?.ToString();
            if (!string.IsNullOrWhiteSpace(hostFromRegistry))
            {
                defaultOrFromRegistryConfigData.Host = hostFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.Host = defaultConfigurationData.Host;
                subKey?.SetValue(Configuration.HOST, defaultConfigurationData.Host);
            }

            var portFromRegistry = (int?)subKey?.GetValue(Configuration.PORT);
            if (portFromRegistry != null && portFromRegistry > 0)
            {
                defaultOrFromRegistryConfigData.Port = (int)portFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.Port = defaultConfigurationData.Port;
                subKey?.SetValue(Configuration.PORT, defaultConfigurationData.Port);
            }

            var clientIdFromRegistry = (int?)subKey?.GetValue(Configuration.CLIENT_ID);
            if (clientIdFromRegistry != null && clientIdFromRegistry > 0)
            {
                defaultOrFromRegistryConfigData.ClientId = (int)clientIdFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.ClientId = defaultConfigurationData.ClientId;
                subKey?.SetValue(Configuration.CLIENT_ID, defaultConfigurationData.ClientId);
            }

            var underlyingSymbolFromRegistry = subKey?.GetValue(Configuration.UNDERLYING_SYMBOL)?.ToString();
            if (!string.IsNullOrWhiteSpace(underlyingSymbolFromRegistry))
            {
                defaultOrFromRegistryConfigData.UnderlyingSymbol = underlyingSymbolFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.UnderlyingSymbol = defaultConfigurationData.UnderlyingSymbol;
                subKey?.SetValue(Configuration.UNDERLYING_SYMBOL, defaultConfigurationData.UnderlyingSymbol);
            }

            var sessionStartFromRegistry = subKey?.GetValue(Configuration.SESSION_START)?.ToString();
            if (!string.IsNullOrWhiteSpace(sessionStartFromRegistry))
            {
                defaultOrFromRegistryConfigData.SessionStart = sessionStartFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.SessionStart = defaultConfigurationData.SessionStart;
                subKey?.SetValue(Configuration.SESSION_START, defaultConfigurationData.SessionStart);
            }

            var sessionEndFromRegistry = subKey?.GetValue(Configuration.SESSION_END)?.ToString();
            if (!string.IsNullOrWhiteSpace(sessionEndFromRegistry))
            {
                defaultOrFromRegistryConfigData.SessionEnd = sessionEndFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.SessionEnd = defaultConfigurationData.SessionEnd;
                subKey?.SetValue(Configuration.SESSION_END, defaultConfigurationData.SessionEnd);
            }

            var dte = (int?)subKey?.GetValue(Configuration.DTE);
            if (dte != null && dte > 0)
            {
                defaultOrFromRegistryConfigData.Dte = (int)dte;
            }
            else
            {
                defaultOrFromRegistryConfigData.Dte = defaultConfigurationData.Dte;
                subKey?.SetValue(Configuration.DTE, defaultConfigurationData.Dte);
            }

            var numberOfStrikes = (int?)subKey?.GetValue(Configuration.NUMBER_OF_STRIKES);
            if (numberOfStrikes != null && numberOfStrikes > 0)
            {
                defaultOrFromRegistryConfigData.NumberOfStrikes = (int)numberOfStrikes;
            }
            else
            {
                defaultOrFromRegistryConfigData.NumberOfStrikes = defaultConfigurationData.NumberOfStrikes;
                subKey?.SetValue(Configuration.NUMBER_OF_STRIKES, defaultConfigurationData.NumberOfStrikes);
            }

            return new ConfigurationData(
                   defaultOrFromRegistryConfigData.Host,
                   defaultOrFromRegistryConfigData.Port,
                   defaultOrFromRegistryConfigData.ClientId,
                   defaultOrFromRegistryConfigData.UnderlyingSymbol,
                   defaultOrFromRegistryConfigData.SessionStart,
                   defaultOrFromRegistryConfigData.SessionEnd,
                   defaultOrFromRegistryConfigData.Dte,
                   defaultOrFromRegistryConfigData.NumberOfStrikes);
        }

        public void WriteConfiguration(ConfigurationData defaultConfigurationData)
        {
            var subKey = _registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER)
                ?? _registryCurrentUser.CreateSubKey(SOFTWARE_SSBHEDGER);

            subKey.SetValue(Configuration.HOST, defaultConfigurationData.Host);
            subKey.SetValue(Configuration.PORT, defaultConfigurationData.Port);
            subKey.SetValue(Configuration.CLIENT_ID, defaultConfigurationData.ClientId);
            subKey.SetValue(Configuration.UNDERLYING_SYMBOL, defaultConfigurationData.UnderlyingSymbol);
            subKey.SetValue(Configuration.SESSION_START, defaultConfigurationData.SessionStart);
            subKey.SetValue(Configuration.SESSION_END, defaultConfigurationData.SessionEnd);
            subKey.SetValue(Configuration.DTE, defaultConfigurationData.Dte);
            subKey.SetValue(Configuration.NUMBER_OF_STRIKES, defaultConfigurationData.NumberOfStrikes);
        }
    }
}
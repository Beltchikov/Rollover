using SsbHedger.Abstractions;
using SsbHedger.Configuration;
using System;
using System.Collections.Generic;

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
        private Dictionary<string, Func<IRegistryKeyAbstraction?, string, object?>> getValueFunctions;
        
        public RegistryManager(IRegistryCurrentUserAbstraction registryCurrentUser)
        {
            _registryCurrentUser = registryCurrentUser;

            getValueFunctions = new Dictionary<string, Func<IRegistryKeyAbstraction?, string, object?>>
            {
                {HOST,  (key, name) => key?.GetValue(name)?.ToString()},
                {PORT,  (key, name) => (int?)key?.GetValue(name)}
            };
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

            string? hostFromRegistry = (string?)GetValue(subKey, HOST);
            defaultOrFromRegistryConfigData = SetValueToReturn(
                defaultOrFromRegistryConfigData,
                hostFromRegistry,
                (v) => !string.IsNullOrWhiteSpace(v?.ToString()),
                defaultConfigurationData);
            SetValueInRegistry(
                subKey,
                hostFromRegistry,
                (v) => !string.IsNullOrWhiteSpace(v?.ToString()),
                defaultConfigurationData);

            //var portFromRegistry = (int?)subKey?.GetValue(PORT);
            //if (portFromRegistry != null && portFromRegistry > 0)
            //{
            //    defaultOrFromRegistryConfigData.Port = (int)portFromRegistry;
            //}
            //else
            //{
            //    defaultOrFromRegistryConfigData.Port = defaultConfigurationData.Port;
            //    subKey?.SetValue(PORT, defaultConfigurationData.Port);
            //}

            int? portFromRegistry = (int?)GetValue(subKey, PORT);
            defaultOrFromRegistryConfigData = SetValueToReturn(
                defaultOrFromRegistryConfigData,
                portFromRegistry,
                (v) => v != null && (int)v > 0,
                defaultConfigurationData);
            SetValueInRegistry(
                subKey,
                hostFromRegistry,
                (v) => !string.IsNullOrWhiteSpace(v?.ToString()),
                defaultConfigurationData);

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

        private static ConfigurationData SetValueToReturn(
            ConfigurationData defaultOrFromRegistryConfigData,
            object? valueFromRegistry,
            Func<object?, bool> validValueFunc,
            ConfigurationData defaultConfigurationData)
        {
            if (validValueFunc(valueFromRegistry))
            {
                defaultOrFromRegistryConfigData.Host = valueFromRegistry;
            }
            else
            {
                defaultOrFromRegistryConfigData.Host = defaultConfigurationData.Host;
        }

            return defaultOrFromRegistryConfigData;
        }

        private void SetValueInRegistry(
           IRegistryKeyAbstraction? subKey,
           string? valueFromRegistry,
           Func<object?, bool> validValueFunc,
           ConfigurationData defaultConfigurationData)
        {
            if (!validValueFunc(valueFromRegistry))
            {
                subKey?.SetValue(HOST, defaultConfigurationData.Host);
            }
        }

        private object? GetValue(IRegistryKeyAbstraction key , string name)
        {
            //return key.GetValue(HOST)?.ToString();

            //Func<IRegistryKeyAbstraction?, string, object?> getValueFunc = (key, name) => key?.GetValue(name)?.ToString();
            //return getValueFunc(key, name);

            return getValueFunctions[name](key, name);
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
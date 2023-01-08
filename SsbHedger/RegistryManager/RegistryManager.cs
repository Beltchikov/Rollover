using SsbHedger.Abstractions;
using SsbHedger.Configuration;
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

            object?[] configValuesFromRegistry = new object?[]
            {
                subKey.GetValue(HOST)?.ToString(),
                (int?)subKey.GetValue(PORT),
                (int?)subKey.GetValue(CLIENT_ID),
                subKey.GetValue(UNDERLYING_SYMBOL)?.ToString(),
                subKey.GetValue(SESSION_START)?.ToString(),
                subKey.GetValue(SESSION_END)?.ToString(),
            };
            object[] configValuesValidated = new object[6];
            for (int i = 0; i < 6; i++)
            {
                var configValue = configValuesFromRegistry[i];
                switch (i)
                {
                    case 0:
                        var configValue0Typed = configValue?.ToString();
                        if (!string.IsNullOrWhiteSpace(configValue0Typed))
                        {
                            configValuesValidated[i] = configValue0Typed;
                        }
                        else
                        {
                            configValuesValidated[i] = defaultConfigurationData.Host;
                            subKey.SetValue(HOST, defaultConfigurationData.Host);
                        }
                        break;
                    case 1:
                        var configValue1Typed = (int?)configValue;
                        if (configValue1Typed != null && configValue1Typed > 0)
                        {
                            configValuesValidated[i] = configValue1Typed;
                        }
                        else
                        {
                            configValuesValidated[i] = defaultConfigurationData.Port;
                            subKey.SetValue(PORT, defaultConfigurationData.Port);
                        }
                        break;
                    case 2:
                        var configValue2Typed = (int?)configValue;
                        if (configValue2Typed != null && configValue2Typed > 0)
                        {
                            configValuesValidated[i] = configValue2Typed;
                        }
                        else
                        {
                            configValuesValidated[i] = defaultConfigurationData.ClientId;
                            subKey.SetValue(CLIENT_ID, defaultConfigurationData.ClientId);
                        }
                        break;
                    case 3:
                        var configValue3Typed = configValue?.ToString();
                        if (!string.IsNullOrWhiteSpace(configValue3Typed))
                        {
                            configValuesValidated[i] = configValue3Typed;
                        }
                        else
                        {
                            configValuesValidated[i] = defaultConfigurationData.UnderlyingSymbol;
                            subKey.SetValue(Configuration.Configuration.UNDERLYING_SYMBOL, defaultConfigurationData.UnderlyingSymbol);
                        }
                        break;
                    case 4:
                        var configValue4Typed = configValue?.ToString();
                        if (!string.IsNullOrWhiteSpace(configValue4Typed))
                        {
                            configValuesValidated[i] = configValue4Typed;
                        }
                        else
                        {
                            configValuesValidated[i] = defaultConfigurationData.SessionStart;
                            subKey.SetValue(Configuration.Configuration.SESSION_START, defaultConfigurationData.SessionStart);
                        }
                        break;
                    case 5:
                        var configValue5Typed = configValue?.ToString();
                        if (!string.IsNullOrWhiteSpace(configValue5Typed))
                        {
                            configValuesValidated[i] = configValue5Typed;
                        }
                        else
                        {
                            configValuesValidated[i] = defaultConfigurationData.SessionEnd;
                            subKey.SetValue(Configuration.Configuration.SESSION_END, defaultConfigurationData.SessionEnd);
                        }
                        break;
                }
            }

            return new ValueTuple<string, int, int, string, string, string>(
                (string)configValuesValidated[0],
                (int)configValuesValidated[1],
                (int)configValuesValidated[2],
                (string)configValuesValidated[3],
                (string)configValuesValidated[4],
                (string)configValuesValidated[5]);
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
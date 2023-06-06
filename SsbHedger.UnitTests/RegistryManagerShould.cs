using AutoFixture.Xunit2;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SsbHedger.Abstractions;
using SsbHedger.SsbConfiguration;

namespace SsbHedger.UnitTests
{
    public class RegistryManagerShould
    {
        const string SOFTWARE_SSBHEDGER = @"SOFTWARE\SsbHedger";
       
        [Theory, AutoNSubstituteData]
        public void ReturnDefaultValuesIfNoValuesInRegistry(
            ConfigurationData defaultConfigurationData,
            [Frozen] IRegistryKeyAbstraction registryKey,
            [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
            RegistryManager sut)
        {
            registryKey.GetValue(Configuration.HOST).ReturnsNull();
            registryKey.GetValue(Configuration.PORT).ReturnsNull();
            registryKey.GetValue(Configuration.CLIENT_ID).ReturnsNull();
            registryKey.GetValue(Configuration.UNDERLYING_SYMBOL).ReturnsNull();
            registryKey.GetValue(Configuration.SESSION_START).ReturnsNull();
            registryKey.GetValue(Configuration.SESSION_END).ReturnsNull();
            registryKey.GetValue(Configuration.DTE).ReturnsNull();
            registryKey.GetValue(Configuration.NUMBER_OF_STRIKES).ReturnsNull();

            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            var configurationData = sut.ReadConfiguration(defaultConfigurationData);

            Assert.Equal(defaultConfigurationData, configurationData);
        }

        [Theory, AutoNSubstituteData]
        public void SaveAndReturnDefaultHostIfHostInRegistryIsEmptyString(
            ConfigurationData defaultConfigurationData,
            [Frozen] IRegistryKeyAbstraction registryKey,
            [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
            RegistryManager sut)
        {
            registryKey.GetValue(Configuration.HOST).Returns(" ");
            registryKey.GetValue(Configuration.PORT).Returns(111);
            registryKey.GetValue(Configuration.CLIENT_ID).Returns(222);
            registryKey.GetValue(Configuration.DTE).Returns(0);
            registryKey.GetValue(Configuration.NUMBER_OF_STRIKES).Returns(10);

            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);
            var (host, _, _, _, _, _,_,_) = sut.ReadConfiguration(defaultConfigurationData);

            Assert.Equal(defaultConfigurationData.Host, host);
            registryKey.Received().SetValue(Configuration.HOST, host);
        }

        [Theory, AutoNSubstituteData]
        public void SaveAndReturnDefaultPortIfPortInRegistryBelowZero(
            ConfigurationData defaultConfigurationData,
            [Frozen] IRegistryKeyAbstraction registryKey,
            [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
            RegistryManager sut)
        {
            registryKey.GetValue(Configuration.HOST).Returns("aaa");
            registryKey.GetValue(Configuration.PORT).Returns(-1);
            registryKey.GetValue(Configuration.CLIENT_ID).Returns(222);
            registryKey.GetValue(Configuration.DTE).Returns(0);
            registryKey.GetValue(Configuration.NUMBER_OF_STRIKES).Returns(10);

            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            var (_, port, _,_,_,_,_,_) = sut.ReadConfiguration(defaultConfigurationData);

            Assert.Equal(defaultConfigurationData.Port, port);
            registryKey.Received().SetValue(Configuration.PORT, port);
        }

        [Theory, AutoNSubstituteData]
        public void SaveAndReturnDefaultClientIdIfClientIdInRegistryBelowZero(
           ConfigurationData defaultConfigurationData,
           [Frozen] IRegistryKeyAbstraction registryKey,
           [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
           RegistryManager sut)
        {
            registryKey.GetValue(Configuration.HOST).Returns("aaa");
            registryKey.GetValue(Configuration.PORT).Returns(4444);
            registryKey.GetValue(Configuration.CLIENT_ID).Returns(-222);
            registryKey.GetValue(Configuration.DTE).Returns(0);
            registryKey.GetValue(Configuration.NUMBER_OF_STRIKES).Returns(10);

            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            var (_, _, clientId,_,_,_,_,_) = sut.ReadConfiguration(defaultConfigurationData);

            Assert.Equal(defaultConfigurationData.ClientId, clientId);
            registryKey.Received().SetValue(SsbConfiguration.Configuration.CLIENT_ID, clientId);
        }

        [Theory, AutoNSubstituteData]
        public void SaveDefaultValuesIfNoValuesInRegistry(
           ConfigurationData defaultConfigurationData,
           [Frozen] IRegistryKeyAbstraction registryKey,
           [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
           RegistryManager sut)
        {
            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).ReturnsNull();
            registryCurrentUser.CreateSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            sut.ReadConfiguration(defaultConfigurationData);

            registryCurrentUser.Received().CreateSubKey(SOFTWARE_SSBHEDGER);
            registryKey.Received().SetValue(SsbConfiguration.Configuration.HOST, defaultConfigurationData.Host);
            registryKey.Received().SetValue(SsbConfiguration.Configuration.PORT, defaultConfigurationData.Port);
            registryKey.Received().SetValue(SsbConfiguration.Configuration.CLIENT_ID, defaultConfigurationData.ClientId);
            registryKey.Received().SetValue(SsbConfiguration.Configuration.UNDERLYING_SYMBOL, defaultConfigurationData.UnderlyingSymbol);
            registryKey.Received().SetValue(SsbConfiguration.Configuration.SESSION_START, defaultConfigurationData.SessionStart);
            registryKey.Received().SetValue(SsbConfiguration.Configuration.SESSION_END, defaultConfigurationData.SessionEnd);
     
        }

        [Theory, AutoNSubstituteData]
        public void ReturnValuesFromRegistry(
            ConfigurationData defaultConfigurationData,
            ConfigurationData configurationDataFromRegistry,
            IRegistryKeyAbstraction registryKey,
            IRegistryCurrentUserAbstraction registryCurrentUser,
            RegistryManager sut)
        {
            registryKey.GetValue(Configuration.HOST)
                .Returns(configurationDataFromRegistry.Host);
            registryKey.GetValue(Configuration.PORT)
                .Returns(configurationDataFromRegistry.Port);
            registryKey.GetValue(Configuration.CLIENT_ID)
                .Returns(configurationDataFromRegistry.ClientId);
            registryKey.GetValue(Configuration.UNDERLYING_SYMBOL)
                .Returns(configurationDataFromRegistry.UnderlyingSymbol);
            registryKey.GetValue(Configuration.SESSION_START)
                .Returns(configurationDataFromRegistry.SessionStart);
            registryKey.GetValue(Configuration.SESSION_END)
                .Returns(configurationDataFromRegistry.SessionEnd);
            registryKey.GetValue(Configuration.DTE)
                .Returns(configurationDataFromRegistry.Dte);
            registryKey.GetValue(Configuration.NUMBER_OF_STRIKES)
                .Returns(configurationDataFromRegistry.NumberOfStrikes);

            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            Reflection.SetFiledValue(sut, "_registryCurrentUser", registryCurrentUser);

            var returnedConfigurationData = sut.ReadConfiguration(defaultConfigurationData);

            Assert.Equal(configurationDataFromRegistry, returnedConfigurationData);
        }

        [Theory, AutoNSubstituteData]
        public void WriteValuesToRegistry(
            ConfigurationData defaultConfigurationData,
            IRegistryKeyAbstraction registryKey,
            [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
            RegistryManager sut)
        {
            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            sut.WriteConfiguration(defaultConfigurationData);

            registryKey.Received().SetValue(SsbConfiguration.Configuration.HOST, defaultConfigurationData.Host);
            registryKey.Received().SetValue(SsbConfiguration.Configuration.PORT, defaultConfigurationData.Port);
            registryKey.Received().SetValue(SsbConfiguration.Configuration.CLIENT_ID, defaultConfigurationData.ClientId);
            registryKey.Received().SetValue(SsbConfiguration.Configuration.UNDERLYING_SYMBOL, defaultConfigurationData.UnderlyingSymbol);
            registryKey.Received().SetValue(SsbConfiguration.Configuration.SESSION_START, defaultConfigurationData.SessionStart);
            registryKey.Received().SetValue(SsbConfiguration.Configuration.SESSION_END, defaultConfigurationData.SessionEnd);
        }
    }
}

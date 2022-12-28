using AutoFixture.Xunit2;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SsbHedger2.Abstractions;

namespace SsbHedger2.UnitTests
{
    public class RegistryManagerShould
    {
        const string SOFTWARE_SSBHEDGER = @"SOFTWARE\SsbHedger";
        const string HOST = @"Host";
        const string PORT = @"Port";
        const string CLIENT_ID = @"ClientId";

        [Theory, AutoNSubstituteData]
        public void ReturnDefaultValuesIfNoValuesInRegistry(
            string defaultHost,
            int defaultPort,
            int defaultClientId,
            [Frozen] IRegistryKeyAbstraction registryKey,
            [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
            RegistryManager sut)
        {
            registryKey.GetValue(HOST).ReturnsNull();
            registryKey.GetValue(PORT).ReturnsNull();
            registryKey.GetValue(CLIENT_ID).ReturnsNull();

            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            var (host, port, clientId) = sut.ReadConfiguration(
                defaultHost,
                defaultPort,
                defaultClientId);

            Assert.Equal(defaultHost, host);
            Assert.Equal(defaultPort, port);
            Assert.Equal(defaultClientId, clientId);
        }

        [Theory, AutoNSubstituteData]
        public void SaveAndReturnDefaultHostIfHostInRegistryIsEmptyString(
            string defaultHost,
            int defaultPort,
            int defaultClientId,
            [Frozen] IRegistryKeyAbstraction registryKey,
            [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
            RegistryManager sut)
        {
            registryKey.GetValue(HOST).Returns(" ");
            registryKey.GetValue(PORT).Returns(111);
            registryKey.GetValue(CLIENT_ID).Returns(222);

            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);
            var (host, _, _) = sut.ReadConfiguration(
                defaultHost,
                defaultPort,
                defaultClientId);

            Assert.Equal(defaultHost, host);
            registryKey.Received().SetValue(HOST, host);
        }

        [Theory, AutoNSubstituteData]
        public void SaveAndReturnDefaultPortIfPortInRegistryBelowZero(
            string defaultHost,
            int defaultPort,
            int defaultClientId,
            [Frozen] IRegistryKeyAbstraction registryKey,
            [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
            RegistryManager sut)
        {
            registryKey.GetValue(HOST).Returns("aaa");
            registryKey.GetValue(PORT).Returns(-1);
            registryKey.GetValue(CLIENT_ID).Returns(222);

            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            var (_, port, _) = sut.ReadConfiguration(
                defaultHost,
                defaultPort,
                defaultClientId);

            Assert.Equal(defaultPort, port);
            registryKey.Received().SetValue(PORT, port);
        }

        [Theory, AutoNSubstituteData]
        public void SaveAndReturnDefaultClientIdIfClientIdInRegistryBelowZero(
           string defaultHost,
           int defaultPort,
           int defaultClientId,
           [Frozen] IRegistryKeyAbstraction registryKey,
           [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
           RegistryManager sut)
        {
            registryKey.GetValue(HOST).Returns("aaa");
            registryKey.GetValue(PORT).Returns(4444);
            registryKey.GetValue(CLIENT_ID).Returns(-222);

            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            var (_, _, clientId) = sut.ReadConfiguration(
                defaultHost,
                defaultPort,
                defaultClientId);

            Assert.Equal(defaultClientId, clientId);
            registryKey.Received().SetValue(CLIENT_ID, clientId);
        }

        [Theory, AutoNSubstituteData]
        public void SaveDefaultValuesIfNoValuesInRegistry(
           string defaultHost,
           int defaultPort,
           int defaultClientId,
           [Frozen] IRegistryKeyAbstraction registryKey,
           [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
           RegistryManager sut)
        {
            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).ReturnsNull();
            registryCurrentUser.CreateSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            sut.ReadConfiguration(
                defaultHost,
                defaultPort,
                defaultClientId);

            registryCurrentUser.Received().CreateSubKey(SOFTWARE_SSBHEDGER);
            registryKey.Received().SetValue(HOST, defaultHost);
            registryKey.Received().SetValue(PORT, defaultPort);
            registryKey.Received().SetValue(CLIENT_ID, defaultClientId);
        }

        [Theory, AutoNSubstituteData]
        public void ReturnValuesFromRegistry(
            string defaultHost,
            int defaultPort,
            int defaultClientId,
            string hostFromRegistry,
            int portFromRegistry,
            int clientIdFromRegistry,
            IRegistryKeyAbstraction registryKey,
            IRegistryCurrentUserAbstraction registryCurrentUser,
            RegistryManager sut)
        {
            registryKey.GetValue(HOST).Returns(hostFromRegistry);
            registryKey.GetValue(PORT).Returns(portFromRegistry);
            registryKey.GetValue(CLIENT_ID).Returns(clientIdFromRegistry);

            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            Reflection.SetFiledValue(sut, "_registryCurrentUser", registryCurrentUser);

            var (host, port, clientId) = sut.ReadConfiguration(
                defaultHost,
                defaultPort,
                defaultClientId);

            Assert.Equal(hostFromRegistry, host);
            Assert.Equal(portFromRegistry, port);
            Assert.Equal(clientIdFromRegistry, clientId);
        }

        [Theory, AutoNSubstituteData]
        public void WriteValuesToRegistry(
            string defaultHost,
            int defaultPort,
            int defaultClientId,
            IRegistryKeyAbstraction registryKey,
            [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
            RegistryManager sut)
        {
            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            sut.WriteConfiguration(
                defaultHost,
                defaultPort,
                defaultClientId);

            registryKey.Received().SetValue(HOST, defaultHost);
            registryKey.Received().SetValue(PORT, defaultPort);
            registryKey.Received().SetValue(CLIENT_ID, defaultClientId);
        }
    }
}

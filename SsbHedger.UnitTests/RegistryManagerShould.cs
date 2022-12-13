using NSubstitute;
using SsbHedger.Abstractions;

namespace SsbHedger.UnitTests
{
    public class RegistryManagerShould
    {
        [Theory, AutoNSubstituteData]
        public void ReturnDefaultValuesIfNoValuesInRegistry(
            string defaultHost,
            int defaultPort,
            int defaultClientId,
            RegistryManager sut)
        {
            var (host, port, clientId) = sut.ReadConfiguration(
                defaultHost,
                defaultPort,
                defaultClientId);

            Assert.Equal(defaultHost, host);
            Assert.Equal(defaultPort, port);
            Assert.Equal(defaultClientId, clientId);
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
            const string SOFTWARE_SSBHEDGER = @"SOFTWARE\SsbHedger";
            const string HOST = @"Host";
            const string PORT = @"Port";
            const string CLIENT_ID = @"ClientId";

            registryKey.GetValue(HOST).Returns(hostFromRegistry);
            registryKey.GetValue(PORT).Returns(portFromRegistry);
            registryKey.GetValue(CLIENT_ID).Returns(clientIdFromRegistry);

            registryCurrentUser.OpenSubKey(SOFTWARE_SSBHEDGER).Returns(registryKey);

            SetFiledValue(sut, "_registryCurrentUser", registryCurrentUser);

            var (host, port, clientId) = sut.ReadConfiguration(
                defaultHost,
                defaultPort,
                defaultClientId);

            Assert.Equal(hostFromRegistry, host);
            Assert.Equal(portFromRegistry, port);
            Assert.Equal(clientIdFromRegistry, clientId);
        }

        private static void SetFiledValue(RegistryManager sut, string fieldName, object value)
        {
            var fieldInfo = sut.GetType().GetField(
                fieldName,
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldInfo?.SetValue(sut, value);
        }

    }
}

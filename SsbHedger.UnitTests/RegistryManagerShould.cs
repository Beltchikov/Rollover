using AutoFixture.Xunit2;
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
            [Frozen] IRegistryCurrentUserAbstraction registryCurrentUser,
            RegistryManager sut)
        {
            var (host, port, clientId ) = sut.ReadConfiguration(
                defaultHost,
                defaultPort,
                defaultClientId);

            Assert.Equal(defaultHost, host);
            Assert.Equal(defaultPort, port);
            Assert.Equal(defaultClientId, clientId);
        }

        [Fact]
        public void ReturnValuesFromRegistry()
        {
            //throw new NotImplementedException();
        }
    }
}

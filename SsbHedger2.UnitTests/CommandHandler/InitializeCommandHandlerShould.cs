using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger2.CommandHandler;

namespace SsbHedger2.UnitTests.CommandHandler
{
    public class InitializeCommandHandlerShould
    {
        [Theory, AutoNSubstituteData]
        public void CallRegistryManager(
            string host,
            int port,
            int clientId,
            [Frozen] IRegistryManager registryManager,
            InitializeCommandHandler sut)
        {
            sut.Handle();
            registryManager.Received().ReadConfiguration(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int>());
        }
    }
}
using AutoFixture.Xunit2;
using NSubstitute;
using Xunit;

namespace Rollover.UnitTests
{
    public class RolloverAgentShould
    {
        [Theory, AutoNSubstituteData]
        public void CallConfigurationManagerCheckConfiguration(
            [Frozen] IConfigurationManager configurationManager,
            RolloverAgent sut)
        {
            sut.Run();
            configurationManager.Received().GetConfiguration();
        }

        // ibClient.ClientSocket.eConnect called

        // EReader inject

        // reader.Start() called

        // ThreadManager.Create called

        // ThreadManager.StartThread

    }
}

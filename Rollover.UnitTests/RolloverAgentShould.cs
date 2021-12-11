using AutoFixture.Xunit2;
using NSubstitute;
using Xunit;

namespace Rollover.UnitTests
{
    public class RolloverAgentShould
    {
        [Theory, AutoNSubstituteData]
        public void CallConfigurationManagerGetConfiguration(
            [Frozen] IConfigurationManager configurationManager,
            [Frozen] IConsoleWrapper inputQueue,
            RolloverAgent sut)
        {
            inputQueue.ReadLine().Returns("q");
            sut.Run();
            configurationManager.Received().GetConfiguration();
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapper(
            [Frozen] IConsoleWrapper inputQueue,
            RolloverAgent sut)
        {
            inputQueue.ReadLine().Returns("SomeInput", "q");
           
            sut.Run();
            inputQueue.Received().ReadLine();
        }

        // ibClient.ClientSocket.eConnect called

        // EReader inject

        // reader.Start() called

        // ThreadManager.Create called

        // ThreadManager.StartThread

    }
}

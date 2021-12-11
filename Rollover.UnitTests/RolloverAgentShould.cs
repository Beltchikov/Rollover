using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Input;
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
            inputQueue.ReadLine().Returns("Q");
            sut.Run();
            configurationManager.Received().GetConfiguration();
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapper(
            [Frozen] IConsoleWrapper consoleWrapper,
            RolloverAgent sut)
        {
            consoleWrapper.ReadLine().Returns("SomeInput", "q");
            sut.Run();
            consoleWrapper.Received().ReadLine();
        }

        [Theory, AutoNSubstituteData]
        public void CallInputQueue(
            [Frozen] IConsoleWrapper consoleWrapper,
            [Frozen] IInputQueue inputQueue,
            RolloverAgent sut)
        {
            consoleWrapper.ReadLine().Returns("SomeInput", "q");
            sut.Run();
            inputQueue.Received().Enqueue(Arg.Any<string>());
        }

        // ibClient.ClientSocket.eConnect called

        // EReader inject

        // reader.Start() called

        // ThreadManager.Create called

        // ThreadManager.StartThread

    }
}

using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using Xunit;

namespace Rollover.UnitTests
{
    public class RolloverAgentShould
    {
        [Theory, AutoNSubstituteData]
        public void CallConfigurationManagerGetConfiguration(
            [Frozen] IConfigurationManager configurationManager,
            [Frozen] IInputQueue inputQueue,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("Q");
            sut.Run();
            configurationManager.Received().GetConfiguration();
        }

        [Theory, AutoNSubstituteData]
        public void CallInputQueue(
            [Frozen] IInputQueue inputQueue,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            inputQueue.Received().Enqueue(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapperReadLine(
            [Frozen] IConsoleWrapper consoleWrapper,
            [Frozen] IInputQueue inputQueue,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            consoleWrapper.Received().ReadLine();
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapperWriteLineWithConfiguration(
            [Frozen] IConsoleWrapper consoleWrapper,
            [Frozen] IInputQueue inputQueue,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("Q");
            sut.Run();
            consoleWrapper.Received().WriteLine(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallRequestSenderRegisterRequestHandlers(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IRequestSender requestSender,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            requestSender.Received().RegisterResponseHandlers();
        }

        [Theory, AutoNSubstituteData]
        public void CallRequestSenderConnect(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IRequestSender requestSender,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            requestSender.Received().Connect(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>());
        }


        // ibClient.ClientSocket.eConnect called

        // EReader inject

        // reader.Start() called

        // ThreadManager.Create called

        // ThreadManager.StartThread

    }
}

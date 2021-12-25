using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using System.Threading;
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
        public void CallrepositoryConnect(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IRepository repository,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            repository.Received().Connect(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>());
        }


        [Theory, AutoNSubstituteData]
        public void CallrepositoryDisconnect(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IRepository repository,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            repository.Received().Disconnect();
        }

        [Theory, AutoNSubstituteData]
        public void CallInputLoopRun(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IInputLoop inputLoop,
           RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            inputLoop.Received().Run(consoleWrapper, inputQueue);
        }
    }
}

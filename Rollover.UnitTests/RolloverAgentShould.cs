using AutoFixture.Xunit2;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using Xunit;

namespace Rollover.UnitTests
{
    public class RolloverAgentShould
    {
        [Theory, AutoNSubstituteData]
        public void CallTwsConnector(
            [Frozen] ITwsConnector twsConnector,
            [Frozen] IInputQueue inputQueue,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("Q");
            sut.Run();
            twsConnector.Received().Connect(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>());
        }

        [Theory, AutoNSubstituteData]
        public void CallRepositoryAllPositions(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IRepository repository,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            repository.Received().AllPositions();
        }

        [Theory, AutoNSubstituteData]
        public void CallPortfolioAdd(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IPortfolio portfolio,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            portfolio.Received().Add(Arg.Any<PositionMessage>());
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapperWriteLine(
            [Frozen] IConsoleWrapper consoleWrapper,
            [Frozen] IInputQueue inputQueue,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("Q");
            sut.Run();
            consoleWrapper.Received().WriteLine(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapperWriteLineEnterSymbolToTrack(
            [Frozen] IConsoleWrapper consoleWrapper,
            [Frozen] IInputQueue inputQueue,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("Q");
            sut.Run();
            consoleWrapper.Received().WriteLine(Constants.ENTER_SYMBOL_TO_TRACK);
        }

        [Theory, AutoNSubstituteData]
        public void CallInputLoopRun(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IIbClientQueue ibClientQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IInputLoop inputLoop,
           RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            inputLoop.Received().Run(consoleWrapper, inputQueue, ibClientQueue);
        }

        [Theory, AutoNSubstituteData]
        public void CallRepositoryDisconnect(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IRepository repository,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            repository.Received().Disconnect();
        }
    }
}

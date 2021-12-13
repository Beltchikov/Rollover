using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Input;
using Xunit;

namespace Rollover.UnitTests
{
    public class InputLoopShould
    {

        [Theory, AutoNSubstituteData]
        public void CallInputQueueDequeue(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run(consoleWrapper, inputQueue);
            inputQueue.Received().Dequeue();
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapperWriteLine(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run(consoleWrapper, inputQueue);
            consoleWrapper.Received().WriteLine(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapperWriteLineIfInputNull(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns((string)null, "q");
            sut.Run(consoleWrapper, inputQueue);
            consoleWrapper.Received().WriteLine("q");
        }
    }
}

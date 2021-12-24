using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using System.Diagnostics;
using Xunit;

namespace Rollover.UnitTests
{
    public class InputLoopShould
    {

        [Theory, AutoNSubstituteData]
        public void CallInputQueueDequeue(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IRepository requestSender,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run(consoleWrapper, inputQueue, requestSender);
            inputQueue.Received().Dequeue();
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapperWriteLine(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IRepository requestSender,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run(consoleWrapper, inputQueue, requestSender);
            consoleWrapper.Received().WriteLine(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapperWriteLineIfInputNull(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IRepository requestSender,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns((string)null, "q");
            sut.Run(consoleWrapper, inputQueue, requestSender);
            consoleWrapper.Received().WriteLine("Goodbye!");
        }

        [Theory, AutoNSubstituteData]
        public void CallInputProcessorConvert(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IInputProcessor inputProcessor,
           [Frozen] IRepository requestSender,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run(consoleWrapper, inputQueue, requestSender);
            inputProcessor.Received().Convert(Arg.Any<string>(), Arg.Any<IRepository>());
        }
    }
}

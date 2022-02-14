using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using System.Diagnostics;
using Rollover.Tests.Shared;
using Xunit;

namespace Rollover.UnitTests
{
    public class InputLoopShould
    {

        [Theory, AutoNSubstituteData]
        public void CallInputQueueDequeue(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IIbClientQueue ibClientQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run(consoleWrapper, inputQueue, ibClientQueue);
            inputQueue.Received().Dequeue();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientQueueDequeue(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IIbClientQueue ibClientQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run(consoleWrapper, inputQueue, ibClientQueue);
            ibClientQueue.Received().Dequeue();
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapperWriteLine(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IIbClientQueue ibClientQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run(consoleWrapper, inputQueue, ibClientQueue);
            consoleWrapper.Received().WriteLine(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapperWriteLineIfInputAndMessageNull(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IIbClientQueue ibClientQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns((string)null, "q");
            ibClientQueue.Dequeue().Returns(null);
            sut.Run(consoleWrapper, inputQueue, ibClientQueue);
            consoleWrapper.Received().WriteLine("Goodbye!");
        }

        [Theory, AutoNSubstituteData]
        public void CallInputProcessorConvert(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IIbClientQueue ibClientQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IInputProcessor inputProcessor,
           [Frozen] IMessageProcessor messageProcessor,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run(consoleWrapper, inputQueue, ibClientQueue);
            inputProcessor.Received().Convert(Arg.Any<string>());
            messageProcessor.Received().ConvertMessage(Arg.Any<object>());
        }
    }
}

using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Input;
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
            consoleWrapper.Received().WriteLine("Goodbye!");
        }

        [Theory, AutoNSubstituteData]
        public void CallOutputHelperConvert(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IOutputHelper outputHelper,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run(consoleWrapper, inputQueue);
            outputHelper.Received().Convert(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallInputQueueDequeueInCheckConnectionMessages(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.CheckConnectionMessages(consoleWrapper, inputQueue, 10000);
            inputQueue.Received().Dequeue();
        }


        [Theory, AutoNSubstituteData]
        public void WaitForConnectionResponseTimeout(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           InputLoop sut)
        {
            int timeout = 10000;
            inputQueue.Dequeue().Returns("SomeInput", "q");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var result = sut.CheckConnectionMessages(consoleWrapper, inputQueue, timeout);
            Assert.True(stopWatch.Elapsed.TotalMilliseconds > timeout);
        }

        [Theory, AutoNSubstituteData]
        public void ReturnFalseFromCheckConnectionMessagesAfterTimeout(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           InputLoop sut)
        {
            int timeout = 10000;
            inputQueue.Dequeue().Returns("SomeInput", "q");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var result = sut.CheckConnectionMessages(consoleWrapper, inputQueue, timeout);
            Assert.False(result);
        }

        [Theory, AutoNSubstituteData]
        public void ReturnTrueFromCheckConnectionMessagesIfSomeResponseContainsConnected(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           InputLoop sut)
        {
            int timeout = 10000;
            inputQueue.Dequeue().Returns("Connected");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var result = sut.CheckConnectionMessages(consoleWrapper, inputQueue, timeout);
            Assert.True(result);
        }
    }
}

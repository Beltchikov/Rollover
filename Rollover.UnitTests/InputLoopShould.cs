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
        public void CallInputProcessorConvert(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IInputProcessor inputProcessor,
           InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run(consoleWrapper, inputQueue);
            inputProcessor.Received().Convert(Arg.Any<string>());
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
           [Frozen] IConnectedCondition connectedCondition,
           InputLoop sut)
        {
            int timeout = 10000;
            inputQueue.Dequeue().Returns("SomeInput", "SomeInput2");
            connectedCondition.IsConnected().Returns(false);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var result = sut.CheckConnectionMessages(consoleWrapper, inputQueue, timeout);
            Assert.True(stopWatch.Elapsed.TotalMilliseconds > timeout);
        }

        [Theory, AutoNSubstituteData]
        public void ReturnFalseFromCheckConnectionMessagesAfterTimeout(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IConnectedCondition connectedCondition,
           InputLoop sut)
        {
            int timeout = 10000;
            inputQueue.Dequeue().Returns("SomeInput", "q");
            connectedCondition.IsConnected().Returns(false);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var result = sut.CheckConnectionMessages(consoleWrapper, inputQueue, timeout);
            Assert.False(result);
        }

        [Theory, AutoNSubstituteData]
        public void CallConnectedConditionInCheckConnectionMessages(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IConnectedCondition connectedCondition,
           InputLoop sut)
        {
            var input = "SomeInput";
            inputQueue.Dequeue().Returns(input);

            sut.CheckConnectionMessages(consoleWrapper, inputQueue, 10000);

            connectedCondition.Received().AddInput(input);
            connectedCondition.Received().IsConnected();
        }

        [Theory, AutoNSubstituteData]
        public void ReturnTrueFromCheckConnectionMessagesIfConnected(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IConsoleWrapper consoleWrapper,
            [Frozen] IConnectedCondition connectedCondition,
            InputLoop sut)
        {
            int timeout = 10000;
            connectedCondition.IsConnected().Returns(true);

            var result = sut.CheckConnectionMessages(consoleWrapper, inputQueue, timeout);
            Assert.True(result);
        }
    }
}

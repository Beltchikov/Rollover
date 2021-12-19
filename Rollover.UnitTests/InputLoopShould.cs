﻿using AutoFixture.Xunit2;
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

        [Theory, AutoNSubstituteData]
        public void CallPortfolioSymbolExists(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IConsoleWrapper consoleWrapper,
            [Frozen] IPortfolio portfolio,
            InputLoop sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run(consoleWrapper, inputQueue);
            portfolio.Received().SymbolExists("SomeInput");
        }

        [Theory, AutoNSubstituteData]
        public void CallTrackedSymbolsSymbolExists(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IConsoleWrapper consoleWrapper,
            [Frozen] IPortfolio portfolio,
            [Frozen] ITrackedSymbols trackedSymbols,
            InputLoop sut)
        {
            var input = "SomeInput";
            inputQueue.Dequeue().Returns(input, "q");
            portfolio.SymbolExists(input).Returns(true);

            sut.Run(consoleWrapper, inputQueue);
            trackedSymbols.Received().SymbolExists(input);
        }

        [Theory, AutoNSubstituteData]
        public void CallTrackedSymbolsAdd(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IConsoleWrapper consoleWrapper,
            [Frozen] IPortfolio portfolio,
            [Frozen] ITrackedSymbols trackedSymbols,
            InputLoop sut)
        {
            var input = "SomeInput";
            inputQueue.Dequeue().Returns(input, "q");
            portfolio.SymbolExists(input).Returns(true);
            trackedSymbols.SymbolExists(input).Returns(false);

            sut.Run(consoleWrapper, inputQueue);
            trackedSymbols.Received().Add(input);
        }
    }
}

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
        public void CallRequestSenderRegisterRequestHandlers(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IRequestSender requestSender,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            requestSender.Received().RegisterResponseHandlers(inputQueue, Arg.Any<SynchronizationContext>());
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


        [Theory, AutoNSubstituteData]
        public void CallRequestSenderDisconnect(
            [Frozen] IInputQueue inputQueue,
            [Frozen] IRequestSender requestSender,
            RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            requestSender.Received().Disconnect();
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

        [Theory, AutoNSubstituteData]
        public void CallInputLoopCheckConnectionMessages(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IInputLoop inputLoop,
           RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            sut.Run();
            inputLoop.Received().CheckConnectionMessages(consoleWrapper, inputQueue);
        }

        [Theory, AutoNSubstituteData]
        public void CallConsoleWrapperWriteLineIfCheckConnectionMessagesReturnsFalse(
           [Frozen] IInputQueue inputQueue,
           [Frozen] IConsoleWrapper consoleWrapper,
           [Frozen] IInputLoop inputLoop,
           RolloverAgent sut)
        {
            inputQueue.Dequeue().Returns("SomeInput", "q");
            inputLoop.CheckConnectionMessages(consoleWrapper, inputQueue)
                .Returns(false);

            sut.Run();
            consoleWrapper.Received().WriteLine("Can not connect!");
        }


        // ibClient.ClientSocket.eConnect called

        // EReader inject

        // reader.Start() called

        // ThreadManager.Create called

        // ThreadManager.StartThread

    }
}

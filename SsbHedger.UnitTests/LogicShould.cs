using AutoFixture.Xunit2;
using IbClient;
using IbClient.messages;
using NSubstitute;
using SsbHedger.Abstractions;
using SsbHedger.ResponseProcessing;
using SsbHedger.ResponseProcessing.Mapper;
using System.Reflection;

namespace SsbHedger.UnitTests
{
    public class LogicShould
    {
        [Theory]
        [InlineData("NextValidId")]
        [InlineData("Error")]
        [InlineData("ManagedAccounts")]
        [InlineData("OpenOrder")]
        [InlineData("OpenOrderEnd")]
        [InlineData("OrderStatus")]
        public void AttachEventHandlers(string eventName)
        {
            var ibClient = IBClient.CreateClient();
            var consoleWrapper = Substitute.For<IConsoleAbstraction>();
            var responseLoop = Substitute.For<IResponseLoop>();
            var responseHandler = Substitute.For<IResponseHandler>();
            var responseMapper = Substitute.For<IResponseMapper>();
            var responseProcessor = Substitute.For<IResponseProcessor>();
      
            responseLoop.When(l => l.Start()).Do(x => { });
            var sut = new Logic(
                ibClient,
                consoleWrapper,
                responseLoop,
                responseHandler,
                responseMapper,
                responseProcessor);

            sut.Execute();

            VerifyDelegateAttachedTo(ibClient, eventName);
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientConnectAndStartReaderThread(
            [Frozen] IIBClient ibClient,
            Logic sut)
        {
            sut.Execute();
            ibClient.Received().ConnectAndStartReaderThread(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int>());
        }

        [Theory, AutoNSubstituteData]
        public void CallNextValidIdHandler(
            ConnectionStatusMessage connectionStatusMessage,
            [Frozen] IIBClient ibClient)
        {
            var wasCalled = false;
            ibClient.NextValidId += (m) => wasCalled = true;

            ibClient.NextValidId += Raise.Event<Action<ConnectionStatusMessage>>(
                connectionStatusMessage);

            Assert.True(wasCalled);
        }


        [Theory, AutoNSubstituteData]
        public void CallResponseLoopStart(
           [Frozen] IResponseLoop responseLoop,
           Logic sut)
        {
            sut.Execute();
            responseLoop.Received().Start();
        }

        [Theory, AutoNSubstituteData]
        public void CallResponseProcessorSetLogic(
            IIBClient ibClient,
            IConsoleAbstraction console,
            IReaderThreadQueue readerQueueMock,
            List<ReqIdAndResponses> messages
            )
        {
            var responseHeandler = new ResponseHandler(readerQueueMock);
            var responseLoop = new ResponseLoop();
            console.ReadKey().Returns(
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false)
                );
            var responseMapper = Substitute.For<IResponseMapper>();
            responseMapper.GetGrouppedResponses().Returns(messages);
            var responseProcessor = Substitute.For<IResponseProcessor>();

            var sut = new Logic(
                ibClient,
                console,
                responseLoop,
                responseHeandler,
                responseMapper,
                responseProcessor);

            sut.Execute();
            responseProcessor.Received().SetLogic(sut);
        }

        [Theory, AutoNSubstituteData]
        public void CallDequeue(
            IIBClient ibClient,
            IConsoleAbstraction console,
            IReaderThreadQueue readerQueueMock,
            List<ReqIdAndResponses> messages
            )
        {
            var responseHeandler = new ResponseHandler(readerQueueMock);
            var responseLoop = new ResponseLoop();
            console.ReadKey().Returns(
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false)
                );
            var responseMapper = Substitute.For<IResponseMapper>();
            responseMapper.GetGrouppedResponses().Returns(messages);
            var responseProcessor = Substitute.For<IResponseProcessor>();
        
            var sut = new Logic(
                ibClient,
                console,
                responseLoop,
                responseHeandler,
                responseMapper, 
                responseProcessor);

            sut.Execute();
            readerQueueMock.Received().Dequeue();
        }

        [Theory, AutoNSubstituteData]
        public void CallResponseMapperAddResponse(
            object message,
            IIBClient ibClient,
            IConsoleAbstraction console,
            IReaderThreadQueue readerQueueMock, 
            List<ReqIdAndResponses> messages)
        {
            readerQueueMock.Dequeue().Returns(message);
            var responseHeandler = new ResponseHandler(readerQueueMock);
            var responseLoop = new ResponseLoop();
            console.ReadKey().Returns(
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false)
                );
            var responseMapper = Substitute.For<IResponseMapper>();
            responseMapper.GetGrouppedResponses().Returns(messages);
            var responseProcessor = Substitute.For<IResponseProcessor>();
  
            var sut = new Logic(
                ibClient,
                console,
                responseLoop,
                responseHeandler,
                responseMapper,
                responseProcessor);

            sut.Execute();
            responseMapper.Received().AddResponse(message);
        }

        [Theory, AutoNSubstituteData]
        public void NotCallResponseMapperAddResponseIfNoMessage(
            IIBClient ibClient,
            IConsoleAbstraction console,
            IReaderThreadQueue readerQueueMock)
        {
            readerQueueMock.Dequeue().Returns(null);
            var responseHeandler = new ResponseHandler(readerQueueMock);
            var responseLoop = new ResponseLoop();
            console.ReadKey().Returns(
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false)
                );
            var responseMapper = Substitute.For<IResponseMapper>();
            var responseProcessor = Substitute.For<IResponseProcessor>();
       
            var sut = new Logic(
                ibClient,
                console,
                responseLoop,
                responseHeandler,
                responseMapper, 
                responseProcessor);

            sut.Execute();
            responseMapper.DidNotReceive().AddResponse(Arg.Any<object>());
        }

        [Theory, AutoNSubstituteData]
        public void CallResponseMapperGetGrouppedResponses(
            object message,
            IIBClient ibClient,
            IConsoleAbstraction console,
            IReaderThreadQueue readerQueueMock,
            List<ReqIdAndResponses> messages)
        {
            readerQueueMock.Dequeue().Returns(message);
            var responseHeandler = new ResponseHandler(readerQueueMock);
            var responseLoop = new ResponseLoop();
            console.ReadKey().Returns(
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false)
                );
            var responseMapper = Substitute.For<IResponseMapper>();
            responseMapper.GetGrouppedResponses().Returns(messages);
            var responseProcessor = Substitute.For<IResponseProcessor>();
  
            var sut = new Logic(
                ibClient,
                console,
                responseLoop,
                responseHeandler,
                responseMapper, 
                responseProcessor);

            sut.Execute();
            responseMapper.Received().GetGrouppedResponses();
        }

        [Theory, AutoNSubstituteData]
        public void CallResponseProcessorForEveryMessage(
            object message,
            IIBClient ibClient,
            IConsoleAbstraction console,
            IReaderThreadQueue readerQueueMock,
            IResponseProcessor responseProcessor,
            List<ReqIdAndResponses> messages)
        {
            readerQueueMock.Dequeue().Returns(message);
            var responseHeandler = new ResponseHandler(readerQueueMock);
            var responseLoop = new ResponseLoop();
            console.ReadKey().Returns(
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false)
                );
            var responseMapper = Substitute.For<IResponseMapper>();
            responseMapper.GetGrouppedResponses().Returns(messages);
      
            var sut = new Logic(
                ibClient,
                console,
                responseLoop,
                responseHeandler,
                responseMapper, 
                responseProcessor);

            sut.Execute();
          
            responseProcessor.Received().Process(messages[0]);
            responseProcessor.Received().Process(messages[1]);
            responseProcessor.Received().Process(messages[2]);
        }

        private void VerifyDelegateAttachedTo(object objectWithEvent, string eventName)
        {
            var allBindings = BindingFlags.IgnoreCase | BindingFlags.Public |
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            var type = objectWithEvent.GetType();
            var fieldInfo = type.GetField(eventName, allBindings);

            var value = fieldInfo?.GetValue(objectWithEvent);

            var handler = value as Delegate;
            Assert.NotNull(handler);
        }

    }

}
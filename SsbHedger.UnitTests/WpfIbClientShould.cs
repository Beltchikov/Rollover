using AutoFixture.Xunit2;
using IbClient;
using IbClient.messages;
using NSubstitute;
using SsbHedger.Abstractions;
using SsbHedger.ResponseProcessing;
using SsbHedger.ResponseProcessing.Mapper;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace SsbHedger.UnitTests
{
    public class WpfIbClientShould
    {
        int _breakLoopAfter = 100;
        DateTime _startTime = DateTime.Now;

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
            var responseLoop = Substitute.For<IResponseLoop>();
            var responseHandler = Substitute.For<IResponseHandler>();
            var responseMapper = Substitute.For<IResponseMapper>();
            var responseProcessor = Substitute.For<IResponseProcessor>();
            var backgroundWorker = Substitute.For<IBackgroundWorkerAbstraction>();
      
            responseLoop.When(l => l.Start()).Do(x => { });
            var sut = new SsbHedger.WpfIbClient.WpfIbClient(
                ibClient,
                responseLoop,
                responseHandler,
                responseMapper,
                responseProcessor,
                backgroundWorker);

            sut.Execute();

            VerifyDelegateAttachedTo(ibClient, eventName);
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientConnectAndStartReaderThread(
            [Frozen] IIBClient ibClient)
        {
            SsbHedger.WpfIbClient.WpfIbClient sut 
                = (SsbHedger.WpfIbClient.WpfIbClient)SsbHedger.WpfIbClient.WpfIbClient
                .Create(() => 1 == 1, (new UIElement()).Dispatcher);
            sut._ibClient = ibClient;
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
        public void CallResponseProcessorSetLogic(
            IIBClient ibClient,
            IReaderThreadQueue readerQueueMock,
            IBackgroundWorkerAbstraction backgroundWorker,
            List<ReqIdAndResponses> messages
            )
        {
            var responseHeandler = new ResponseHandler(readerQueueMock);

            var responseLoop = new ResponseLoop();
            responseLoop.BreakCondition =
                () => (DateTime.Now - _startTime).Milliseconds > _breakLoopAfter;

            var responseMapper = Substitute.For<IResponseMapper>();
            responseMapper.GetGrouppedResponses().Returns(messages);
            var responseProcessor = Substitute.For<IResponseProcessor>();
            
            var sut = new SsbHedger.WpfIbClient.WpfIbClient(
                ibClient,
                responseLoop,
                responseHeandler,
                responseMapper,
                responseProcessor, 
                backgroundWorker);

            sut.Execute();
            responseProcessor.Received().SetLogic(sut);
        }

        [Theory, AutoNSubstituteData]
        public void NotCallResponseMapperAddResponseIfNoMessage(
            IIBClient ibClient,
            IReaderThreadQueue readerQueueMock,
            IBackgroundWorkerAbstraction backgroundWorker)
        {
            readerQueueMock.Dequeue().Returns(null);
            var responseHeandler = new ResponseHandler(readerQueueMock);

            var responseLoop = new ResponseLoop();
            responseLoop.BreakCondition =
                () => (DateTime.Now - _startTime).Milliseconds > _breakLoopAfter;

            var responseMapper = Substitute.For<IResponseMapper>();
            var responseProcessor = Substitute.For<IResponseProcessor>();
            
            var sut = new SsbHedger.WpfIbClient.WpfIbClient(
                ibClient,
                responseLoop,
                responseHeandler,
                responseMapper, 
                responseProcessor, 
                backgroundWorker );

            sut.Execute();
            responseMapper.DidNotReceive().AddResponse(Arg.Any<object>());
        }

        [Theory, AutoNSubstituteData]
        public void CallBackgroundWorker(
            object message,
            IIBClient ibClient,
            IReaderThreadQueue readerQueueMock,
            IResponseProcessor responseProcessor,
            IBackgroundWorkerAbstraction backgroundWorker,
            List<ReqIdAndResponses> messages)
        {
            readerQueueMock.Dequeue().Returns(message);
            var responseHeandler = new ResponseHandler(readerQueueMock);

            var responseLoop = new ResponseLoop();
            responseLoop.BreakCondition =
                () => (DateTime.Now - _startTime).Milliseconds > _breakLoopAfter;

            var responseMapper = Substitute.For<IResponseMapper>();
            responseMapper.GetGrouppedResponses().Returns(messages);
            
            var sut = new SsbHedger.WpfIbClient.WpfIbClient(
                ibClient,
                responseLoop,
                responseHeandler,
                responseMapper,
                responseProcessor,
                backgroundWorker);

            sut.Execute();

            backgroundWorker.Received().RunWorkerAsync();   
        }

        [Theory, AutoNSubstituteData]
        public void CallResponseLoopStart(
            object message,
            IIBClient ibClient,
            IResponseLoop responseLoop,
            IReaderThreadQueue readerQueueMock,
            IResponseProcessor responseProcessor,
            List<ReqIdAndResponses> messages)
        {
            readerQueueMock.Dequeue().Returns(message);
            var responseHeandler = new ResponseHandler(readerQueueMock);

            responseLoop.BreakCondition =
                () => (DateTime.Now - _startTime).Milliseconds > _breakLoopAfter;

            IBackgroundWorkerAbstraction backgroundWorker = new BackgroundWorkerAbstraction();
            backgroundWorker.SetDoWorkEventHandler((s, e) =>
            {
                responseLoop.Start();
            });

            var responseMapper = Substitute.For<IResponseMapper>();
            responseMapper.GetGrouppedResponses().Returns(messages);

            var sut = new SsbHedger.WpfIbClient.WpfIbClient(
                ibClient,
                responseLoop,
                responseHeandler,
                responseMapper,
                responseProcessor,
                backgroundWorker);

            sut.Execute();

            responseLoop.Received().Start();
        }

        [Theory, AutoNSubstituteData]
        public void CallDequeue(
            object message,
            IIBClient ibClient,
            IReaderThreadQueue readerQueueMock,
            IResponseProcessor responseProcessor,
            List<ReqIdAndResponses> messages)
        {
            readerQueueMock.Dequeue().Returns(message);
            var responseHandler = new ResponseHandler(readerQueueMock);

            IResponseLoop responseLoop = new ResponseLoop();
            responseLoop.BreakCondition =
                () => (DateTime.Now - _startTime).Milliseconds > _breakLoopAfter;

            IBackgroundWorkerAbstraction backgroundWorker = new BackgroundWorkerAbstraction();
            backgroundWorker.SetDoWorkEventHandler((s, e) =>
            {
                responseLoop.Start();
            });

            var responseMapper = Substitute.For<IResponseMapper>();
            responseMapper.GetGrouppedResponses().Returns(messages);

            var sut = new SsbHedger.WpfIbClient.WpfIbClient(
                ibClient,
                responseLoop,
                responseHandler,
                responseMapper,
                responseProcessor,
                backgroundWorker);

            sut.Execute();

            Thread.Sleep(_breakLoopAfter);
            readerQueueMock.Received().Dequeue();
        }

        [Theory, AutoNSubstituteData]
        public void CallResponseMapperAddResponse(
            object message,
            IIBClient ibClient,
            IReaderThreadQueue readerQueueMock,
            IResponseProcessor responseProcessor,
            IResponseMapper responseMapper,
            List<ReqIdAndResponses> messages)
        {
            readerQueueMock.Dequeue().Returns(message);
            var responseHandler = new ResponseHandler(readerQueueMock);

            IResponseLoop responseLoop = new ResponseLoop();
            responseLoop.BreakCondition =
                () => (DateTime.Now - _startTime).Milliseconds > _breakLoopAfter;

            IBackgroundWorkerAbstraction backgroundWorker = new BackgroundWorkerAbstraction();
            backgroundWorker.SetDoWorkEventHandler((s, e) =>
            {
                responseLoop.Start();
            });

            responseMapper.GetGrouppedResponses().Returns(messages);

            var sut = new SsbHedger.WpfIbClient.WpfIbClient(
                ibClient,
                responseLoop,
                responseHandler,
                responseMapper,
                responseProcessor,
                backgroundWorker);

            sut.Execute();

            Thread.Sleep(_breakLoopAfter);
            responseMapper.Received().AddResponse(Arg.Any<object>());
        }

        [Theory, AutoNSubstituteData]
        public void CallResponseMapperGetGrouppedResponses(
            object message,
            IIBClient ibClient,
            IReaderThreadQueue readerQueueMock,
            IResponseProcessor responseProcessor,
            IResponseMapper responseMapper,
            List<ReqIdAndResponses> messages)
        {
            readerQueueMock.Dequeue().Returns(message);
            var responseHandler = new ResponseHandler(readerQueueMock);

            IResponseLoop responseLoop = new ResponseLoop();
            responseLoop.BreakCondition =
                () => (DateTime.Now - _startTime).Milliseconds > _breakLoopAfter;

            IBackgroundWorkerAbstraction backgroundWorker = new BackgroundWorkerAbstraction();
            backgroundWorker.SetDoWorkEventHandler((s, e) =>
            {
                responseLoop.Start();
            });

            responseMapper.GetGrouppedResponses().Returns(messages);

            var sut = new SsbHedger.WpfIbClient.WpfIbClient(
                ibClient,
                responseLoop,
                responseHandler,
                responseMapper,
                responseProcessor,
                backgroundWorker);

            sut.Execute();

            Thread.Sleep(_breakLoopAfter);
            responseMapper.Received().GetGrouppedResponses();
        }

        [Theory, AutoNSubstituteData]
        public void CallResponseProcessorForEveryMessage(
            object message,
            IIBClient ibClient,
            IReaderThreadQueue readerQueueMock,
            IResponseProcessor responseProcessor,
            IResponseMapper responseMapper,
            List<ReqIdAndResponses> messages)
        {
            readerQueueMock.Dequeue().Returns(message);
            var responseHandler = new ResponseHandler(readerQueueMock);

            IResponseLoop responseLoop = new ResponseLoop();
            responseLoop.BreakCondition =
                () => (DateTime.Now - _startTime).Milliseconds > _breakLoopAfter;

            IBackgroundWorkerAbstraction backgroundWorker = new BackgroundWorkerAbstraction();
            backgroundWorker.SetDoWorkEventHandler((s, e) =>
            {
                responseLoop.Start();
            });

            responseMapper.GetGrouppedResponses().Returns(messages);

            var sut = new SsbHedger.WpfIbClient.WpfIbClient(
                ibClient,
                responseLoop,
                responseHandler,
                responseMapper,
                responseProcessor,
                backgroundWorker);

            sut.Execute();

            Thread.Sleep(_breakLoopAfter);
            responseProcessor.Received().Process(Arg.Any<ReqIdAndResponses>());
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
using AutoFixture.Xunit2;
using IbClient.messages;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using SsbHedger.Abstractions;
using SsbHedger.ResponseProcessing;
using SsbHedger.WpfIbClient;
using System.Windows;

namespace SsbHedger.UnitTests.ResponseProcessing
{
    public class ResponseHandlerShould
    {

        [Theory, AutoNSubstituteData]
        public void CallEnqueueOnError(
            int reqId, int code, string message, Exception exception,
            [Frozen] IReaderThreadQueue queue,
            ResponseHandler sut)
        {
            sut.OnError(reqId, code, message, exception);
            queue.Received().Enqueue(Arg.Is<object>(a =>
                a.GetType() == typeof(ErrorInfo)
                && ((ErrorInfo)a).ReqId == reqId
                && ((ErrorInfo)a).Code == code
                && ((ErrorInfo)a).Message == message
            ));
        }

        [Theory, AutoNSubstituteData]
        public void CallEnqueueOnManagedAccounts(
            ManagedAccountsMessage managedAccountsMessage,
            [Frozen] IReaderThreadQueue queue,
            ResponseHandler sut)
        {
            sut.OnManagedAccounts(managedAccountsMessage);
            queue.Received().Enqueue(Arg.Is<object>(a =>
                a.GetType() == typeof(ManagedAccountsMessage)
                && ((ManagedAccountsMessage)a).ManagedAccounts.Count()
                    == managedAccountsMessage.ManagedAccounts.Count()
            ));
        }

        [Theory, AutoNSubstituteData]
        public void CallEnqueueOnNextValidId(
            ConnectionStatusMessage connectionStatusMessage,
            [Frozen] IReaderThreadQueue queue,
            ResponseHandler sut)
        {
            sut.OnNextValidId(connectionStatusMessage);
            queue.Received().Enqueue(Arg.Is<object>(a =>
                a.GetType() == typeof(ConnectionStatusMessage)
                && ((ConnectionStatusMessage)a).IsConnected
                    == connectionStatusMessage.IsConnected
            ));
        }

        [Theory, AutoNSubstituteData]
        public void CallEnqueueOnOpenOrder(
            OpenOrderMessage openOrderMessage,
            [Frozen] IReaderThreadQueue queue,
            ResponseHandler sut)
        {
            sut.OnOpenOrder(openOrderMessage);
            queue.Received().Enqueue(Arg.Is<object>(a =>
                a.GetType() == typeof(OpenOrderMessage)
                && ((OpenOrderMessage)a).Contract.ConId
                    == openOrderMessage.Contract.ConId
                && ((OpenOrderMessage)a).Order.OrderId
                    == openOrderMessage.Order.OrderId
                && ((OpenOrderMessage)a).OrderState
                    == openOrderMessage.OrderState
            ));
        }

        [Theory, AutoNSubstituteData]
        public void CallEnqueueOnOpenOrderEnd(
            [Frozen] IReaderThreadQueue queue,
            ResponseHandler sut)
        {
            sut.OnOpenOrderEnd();
            queue.Received().Enqueue(Arg.Is<object>(a =>
                a.GetType() == typeof(string)
                && (string)a == "OpenOrderEnd"
            ));
        }

        [Theory, AutoNSubstituteData]
        public void CallEnqueueOnOrderStatus(
            OrderStatusMessage orderStatusMessage,
            [Frozen] IReaderThreadQueue queue,
            ResponseHandler sut)
        {
            sut.OnOrderStatus(orderStatusMessage);
            queue.Received().Enqueue(Arg.Is<object>(a =>
                a.GetType() == typeof(OrderStatusMessage)
                && ((OrderStatusMessage)a).Status
                    == orderStatusMessage.Status
            ));
        }

        [Theory, AutoNSubstituteData]
        public void CallDequeueOnDequeue(
            [Frozen] IReaderThreadQueue queue,
            ResponseHandler sut)
        {
            sut.Dequeue();
            queue.Received().Dequeue();
        }

        [Theory, AutoNSubstituteData]
        public void CallDequeueOnHandleNextMessage(
            [Frozen] IReaderThreadQueue queue,
            ResponseHandler sut)
        {
            sut.HandleNextMessage();
            queue.Received().Dequeue();
        }

        [Theory, AutoNSubstituteData]
        public void CallIDispatcheInvoke(
            ErrorInfo message,
            [Frozen] IReaderThreadQueue queue,
            [Frozen] IDispatcherAbstraction dispatcherAbstraction,
            [Frozen] IWpfIbClient client,
            ResponseHandler sut)
        {
            queue.Dequeue().Returns(message);
            sut.SetClient(client);
            sut.HandleNextMessage();

            dispatcherAbstraction.Received().Invoke(Arg.Any<Action>());
        }

        [Fact]
        public void TriggerErrorEventOnClient()
        {
            ErrorInfo message = new(1, 3, "err", null);

            IReaderThreadQueue queue = Substitute.For<IReaderThreadQueue>();
            queue.Dequeue().Returns(message);

            var dispatcher = (new UIElement()).Dispatcher;
            var dispatcherAbstraction = new DispatcherAbstraction(dispatcher);
            IWpfIbClient client = Substitute.For<IWpfIbClient>();

            ResponseHandler sut = new(queue, dispatcherAbstraction);
            sut.SetClient(client);
            sut.HandleNextMessage();

            client.Received().InvokeError(message.ReqId, 
                $"Code:{message.Code} Message:{message.Message} Exception:{message.Exception}");
        }

        [Fact]
        public void TriggerNextValidIdEventOnClient()
        {
            ConnectionStatusMessage message = new ConnectionStatusMessage(true);

            IReaderThreadQueue queue = Substitute.For<IReaderThreadQueue>();
            queue.Dequeue().Returns(message);

            var dispatcher = (new UIElement()).Dispatcher;
            var dispatcherAbstraction = new DispatcherAbstraction(dispatcher);
            IWpfIbClient client = Substitute.For<IWpfIbClient>();

            ResponseHandler sut = new(queue, dispatcherAbstraction);
            sut.SetClient(client);
            sut.HandleNextMessage();

            client.Received().InvokeNextValidId(message);
        }

        [Fact]
        public void TriggerManagedAccountsOnClient()
        {
            ManagedAccountsMessage message = new ManagedAccountsMessage("acc1, acc1");

            IReaderThreadQueue queue = Substitute.For<IReaderThreadQueue>();
            queue.Dequeue().Returns(message);

            var dispatcher = (new UIElement()).Dispatcher;
            var dispatcherAbstraction = new DispatcherAbstraction(dispatcher);
            IWpfIbClient client = Substitute.For<IWpfIbClient>();

            ResponseHandler sut = new(queue, dispatcherAbstraction);
            sut.SetClient(client);
            sut.HandleNextMessage();

            client.Received().InvokeManagedAccounts(message);
        }
    }
}

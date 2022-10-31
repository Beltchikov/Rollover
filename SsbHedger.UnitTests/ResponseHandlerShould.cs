using AutoFixture.Xunit2;
using IbClient.messages;
using NSubstitute;
using SsbHedger.ResponseProcessing;

namespace SsbHedger.UnitTests
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
    }
}

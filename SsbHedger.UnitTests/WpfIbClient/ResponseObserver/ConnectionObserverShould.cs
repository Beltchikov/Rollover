using NSubstitute;
using SsbHedger.WpfIbClient.ResponseObservers;

namespace SsbHedger.UnitTests.WpfIbClient.ResponseObserver
{
    public class ConnectionObserverShould
    {
        [Theory, AutoNSubstituteData]
        public void CallIbClientSubscribeForConnection(
            IObservable<Connection> connectionObservable,
            ConnectionObserver sut)
        {
            sut.Subscribe(connectionObservable);
            connectionObservable.Received().Subscribe(
                Arg.Any<IObserver<Connection>>());
        }

        [Theory, AutoNSubstituteData]
        public void CallUnsubscriberConnectionDispose(
            IDisposable unsubscriber,
            IObservable<Connection> connectionObservable,
            ConnectionObserver sut)
        {
            sut.Subscribe(connectionObservable);
            sut._unsubscriberConnection = unsubscriber;
            sut.UnsubscribeForConnection();

            unsubscriber.Received().Dispose();
        }
    }
}

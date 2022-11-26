using NSubstitute;
using SsbHedger.WpfIbClient.ResponseObservers;

namespace SsbHedger.UnitTests.WpfIbClient.ResponseObserver
{
    public class ConnectionObserverShould
    {
        [Theory, AutoNSubstituteData]
        public void CallIbClientSubscribe(
            IObservable<Connection> connectionObservable,
            ConnectionObserver sut)
        {
            sut.Subscribe(connectionObservable);
            connectionObservable.Received().Subscribe(
                Arg.Any<IObserver<Connection>>());
        }

        [Theory, AutoNSubstituteData]
        public void CallUnsubscriberDispose(
            IDisposable unsubscriber,
            IObservable<Connection> connectionObservable,
            ConnectionObserver sut)
        {
            sut.Subscribe(connectionObservable);
            sut._unsubscriber = unsubscriber;
            sut.Unsubscribe();

            unsubscriber.Received().Dispose();
        }
    }
}

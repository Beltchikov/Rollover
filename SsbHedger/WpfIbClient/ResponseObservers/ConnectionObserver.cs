using System;

namespace SsbHedger.WpfIbClient.ResponseObservers
{
    public class ConnectionObserver : IObserver<Connection>
    {
        internal IDisposable _unsubscriber;

        public ConnectionObserver()
        {
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Connection value)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(IObservable<Connection> ibClient)
        {
            _unsubscriber = ibClient.Subscribe(this);
        }

        public void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }
    }
}
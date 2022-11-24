using System;

namespace SsbHedger.ResponseObservers
{
    public class ConnectionObserver : IObserver<string>
    {
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

        public void OnNext(string value)
        {
            throw new NotImplementedException();
        }
    }
}
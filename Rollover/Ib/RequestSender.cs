using System.Threading;

namespace Rollover.Ib
{
    public class RequestSender : IRequestSender
    {
        private IIbClientWrapper _ibClient;

        public RequestSender(IIbClientWrapper ibClient)
        {
            _ibClient = ibClient;
        }

        public void Connect(string host, int port, int clientId)
        {
            _ibClient.Connect(host, port, clientId);

            var reader = _ibClient.ReaderFactory();
            reader.Start();

            new Thread(() =>
            {
                while (_ibClient.IsConnected())
                {
                    _ibClient.WaitForSignal();
                    reader.processMsgs();
                }
            })
            { IsBackground = true }
            .Start();
        }

        public void RegisterResponseHandlers()
        {
            _ibClient.Error += ResponseHandlers.OnError;
            _ibClient.NextValidId += ResponseHandlers.NextValidId;
            _ibClient.ManagedAccounts += ResponseHandlers.ManagedAccounts;
        }

        
    }
}

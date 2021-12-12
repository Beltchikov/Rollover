using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            //string host = txtHost.Text;
            //int port = Int32.Parse(txtPort.Text);
            //int clientId = Int32.Parse(txtClientId.Text);

            //ibClient.ClientSocket.eConnect(host, port, ibClient.ClientId);

            //// The EReader Thread
            //var reader = new EReader(ibClient.ClientSocket, signal);
            //reader.Start();
            //new Thread(() =>
            //{
            //    while (ibClient.ClientSocket.IsConnected())
            //    {
            //        signal.waitForSignal();
            //        reader.processMsgs();
            //    }
            //})
            //{ IsBackground = true }
            //.Start();
        }

        public void RegisterResponseHandlers()
        {
            _ibClient.Error += ResponseHandlers.OnError;
            _ibClient.NextValidId += ResponseHandlers.NextValidId;
            _ibClient.ManagedAccounts += ResponseHandlers.ManagedAccounts;
        }

        
    }
}

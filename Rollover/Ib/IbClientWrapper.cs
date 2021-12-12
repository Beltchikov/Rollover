using IBApi;
using IBSampleApp;
using IBSampleApp.messages;
using System;

namespace Rollover.Ib
{
    public class IbClientWrapper : IIbClientWrapper
    {
        private EReaderMonitorSignal _signal;
        private IBClient _ibClient;

        public event Action<int, int, string, Exception> Error;
        public event Action<ConnectionStatusMessage> NextValidId;
        public event Action<ManagedAccountsMessage> ManagedAccounts;

        public IbClientWrapper()
        {
            _signal = new EReaderMonitorSignal();
            _ibClient = new IBClient(_signal);
        }

        public void Connect(string host, int port, int clientId)
        {
            _ibClient.ClientSocket.eConnect(host, port, clientId);

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

            //EnableControls(true);
        }
    }
}

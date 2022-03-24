using System;
using UsMoversOpening.Configuration;
using UsMoversOpening.IBApi;
using UsMoversOpening.Threading;

namespace UsMoversOpening
{
    public class UmoAgent : IUmoAgent
    {
        private IConfigurationManager _configurationManager;
        private IIbClientWrapper _ibClientWrapper;
        private IStocksBuyer _stocksBuyer;
        private bool _ordersSent;

        public UmoAgent(
            IConfigurationManager configurationManager,
            IIbClientWrapper ibClientWrapper,
            IStocksBuyer stocksBuyer)
        {
            _configurationManager = configurationManager;
            _ibClientWrapper = ibClientWrapper;
            _stocksBuyer = stocksBuyer;

            _ibClientWrapper.OnErrorFunction = OnError;
        }

        private void OnError(int arg1, int arg2, string arg3, Exception arg4)
        {
            throw new NotImplementedException();
        }

        public void Run(IThreadSpawner threadSpawner, IThreadWrapper inputThread, IThreadWrapper ibThread)
        {
            inputThread.Start();
            var configuration = _configurationManager.GetConfiguration();
            _ibClientWrapper.eConnect(configuration.Host, configuration.Port, configuration.ClientId);
            ibThread.Start();

            //try
            //{
            //    string host = txtHost.Text;
            //    int port = Int32.Parse(txtPort.Text);
            //    int clientId = Int32.Parse(txtClientId.Text);

            //    ibClient.ClientSocket.eConnect(host, port, ibClient.ClientId);

            //    // The EReader Thread
            //    var reader = new EReader(ibClient.ClientSocket, signal);
            //    reader.Start();
            //    new Thread(() =>
            //    {
            //        while (ibClient.ClientSocket.IsConnected())
            //        {
            //            signal.waitForSignal();
            //            reader.processMsgs();
            //        }
            //    })
            //    { IsBackground = true }
            //    .Start();

            //    EnableControls(true);
            //}
            //catch (Exception ex)
            //{
            //    txtMessage.Text += Environment.NewLine + ex.ToString();
            //}

            // Working loop
            while (!threadSpawner.ExitFlagInputThread)
            {
                if (_stocksBuyer.Triggered(configuration.TimeToBuy) && !_ordersSent)
                {
                    _ordersSent = _stocksBuyer.SendOrders();
                }
                //_consoleWrapper.WriteLine("In loop");
            }
        }
    }
}
using UsMoversOpening.Configuration;
using UsMoversOpening.Threading;

namespace UsMoversOpening
{
    public class UmoAgent : IUmoAgent
    {
        private IConfigurationManager _configurationManager;
        private IStocksBuyer _stocksBuyer;
        private bool _ordersSent;

        public UmoAgent(
            IConfigurationManager configurationManager,
            IStocksBuyer stocksBuyer)
        {
            _configurationManager = configurationManager;
            _stocksBuyer = stocksBuyer;
        }

        public void Run(IThreadSpawner threadSpawner, IThreadWrapper inputThread, IThreadWrapper ibThread)
        {
            inputThread.Start();
            var configuration = _configurationManager.GetConfiguration();

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
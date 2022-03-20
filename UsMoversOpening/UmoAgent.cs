using System;
using System.Linq;
using System.Threading;
using UsMoversOpening.Configuration;
using UsMoversOpening.Helper;

namespace UsMoversOpening
{
    public class UmoAgent : IUmoAgent
    {
        private IConfigurationManager _configurationManager;
        private IStocksBuyer _stocksBuyer;
        private IUmoTimer _iUmoTimer;
        private IConsoleWrapper _consoleWrapper;
        private bool _ordersSent;
        private bool _exitFlag;

        public UmoAgent(
            IConfigurationManager configurationManager,
            IStocksBuyer stocksBuyer,
            IUmoTimer iUmoTimer, 
            IConsoleWrapper consoleWrapper)
        {
            _configurationManager = configurationManager;
            _stocksBuyer = stocksBuyer;
            _iUmoTimer = iUmoTimer;
            _consoleWrapper = consoleWrapper;
        }

        public void Run()
        {
            // Start input thread
            new Thread(() => {
                while (true) 
                { 
                    if(_consoleWrapper.ReadLine() == "q")
                    {
                        _exitFlag = true;
                    }
                }
            })
            { IsBackground = true }
            .Start();

            // Read configuration
            var configuration = _configurationManager.GetConfiguration();

            // Working loop
            while (!_exitFlag)
            {
                if (_iUmoTimer.Triggered(configuration.TimeToBuy) && !_ordersSent)
                {
                    _ordersSent = _stocksBuyer.SendOrders();
                }
                //_consoleWrapper.WriteLine("In loop");
            }

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
        }
    }
}
using System;
using System.Linq;
using UsMoversOpening.Configuration;

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

        public void Run()
        {
            var configuration = _configurationManager.GetConfiguration();

            if (TimeToBuyTriggered(configuration.TimeToBuy) && !_ordersSent)
            {
                _ordersSent = _stocksBuyer.SendOrders();
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

        private bool TimeToBuyTriggered(string timeToBuyString)
        {
            var timeToBuyArray= timeToBuyString.Split(":");
            var hourToBuy = Convert.ToInt32(timeToBuyArray.First());
            var minuteToBuy = Convert.ToInt32(timeToBuyArray.Last());

            var timeToBuy = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                hourToBuy,
                minuteToBuy,
                0);

            return timeToBuy > DateTime.Now;
        }
    }
}
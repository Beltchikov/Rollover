using UsMoversOpening.Configuration;

namespace UsMoversOpening
{
    public class UmoAgent : IUmoAgent
    {
        private IConfigurationManager _configurationManager;

        public UmoAgent(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        public void Run()
        {
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
        }
    }
}
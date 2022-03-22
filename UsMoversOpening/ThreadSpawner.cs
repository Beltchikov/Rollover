using System;
using System.Diagnostics.CodeAnalysis;
using UsMoversOpening.Configuration;

namespace UsMoversOpening
{
    public class ThreadSpawner : IThreadSpawner
    {
        private IUmoAgent _umoAgent;
        private IConfigurationManager _configurationManager;

        public ThreadSpawner(IUmoAgent umoAgent, IConfigurationManager configurationManager)
        {
            _umoAgent = umoAgent;
            _configurationManager = configurationManager;
        }

        public bool ExitFlagInputThread { get; set; }

        public void Run()
        {
            var configuration = _configurationManager.GetConfiguration();
            string host = configuration.Host;
            int port = configuration.Port;
            int clientId = configuration.ClientId;

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

            var inputThread = InputThreadFactory();
            _umoAgent.Run(this, inputThread);
        }

        [ExcludeFromCodeCoverage]
        private ThreadWrapper InputThreadFactory()
        {
            return new ThreadWrapper(
                () =>
                {
                    while (true)
                    {
                        if (Console.ReadLine() == "q")
                        {
                            ExitFlagInputThread = true;
                        }
                    }
                },
                true);
        }
    }
}

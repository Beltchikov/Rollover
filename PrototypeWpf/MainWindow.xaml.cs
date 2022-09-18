using System;
using System.Windows;
//using TwsApi;

namespace PrototypeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private IbClient _ibClient;
        
        public MainWindow()
        {
            //_ibClient = new IbClient();

            InitializeComponent();
        }

        private void btConnect_Click(object sender, RoutedEventArgs e)
        {
            string host = tbHost.Text;
            int port = Int32.Parse(tbPort.Text);
            int clientId = Int32.Parse(tbClientId.Text);

            //_ibClient.ClientSocket.eConnect(host, port, clientId);

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

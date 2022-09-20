using IBApi;
using System;
using System.Threading;
using System.Windows;
using TwsApi;

namespace PrototypeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IBClient _ibClient;
        private EReaderMonitorSignal _signal;

        public MainWindow()
        {
            _signal = new EReaderMonitorSignal();
            _ibClient = new IBClient(_signal);

            InitializeComponent();
        }

        private void btConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ibClient.ClientSocket.eConnect(
                       tbHost.Text,
                       Int32.Parse(tbPort.Text),
                       Int32.Parse(tbClientId.Text));

                // The EReader Thread
                var reader = new EReader(_ibClient.ClientSocket, _signal);
                reader.Start();
                new Thread(() =>
                {
                    while (_ibClient.ClientSocket.IsConnected())
                    {
                        _signal.waitForSignal();
                        reader.processMsgs();
                    }
                })
                { IsBackground = true }
                .Start();
            }
            catch (Exception ex)
            {
                tbMessages.Text = ex.ToString() 
                    + (String.IsNullOrWhiteSpace(tbMessages.Text)
                        ? String.Empty
                        : Environment.NewLine + tbMessages.Text);
            }
        }
    }
}

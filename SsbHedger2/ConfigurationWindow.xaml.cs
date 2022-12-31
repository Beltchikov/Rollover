using System.Windows;

namespace SsbHedger
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow(string host, int port, int clientId)
        {
            InitializeComponent();

            txtHost.Text = host;
            txtPort.Text = port.ToString();
            txtClientId.Text = clientId.ToString();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btDone_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
//using static System.Net.Mime.MediaTypeNames;

namespace SsbHedger
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        string _initialHost;
        int _initialPort;
        int _initialClientId;

        Regex _regexDigits = new ("^[0-9]*$");


        public ConfigurationWindow(string host, int port, int clientId)
        {
            InitializeComponent();

            txtHost.Text = _initialHost = host;
            txtPort.Text = (_initialPort = port).ToString();
            txtClientId.Text = (_initialClientId = clientId).ToString();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btDone_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void txtHost_TextChanged(object sender, TextChangedEventArgs e)
        {
            btDone.IsEnabled = ConfigurationIsUpdated(((TextBox)sender), txtPort, txtClientId);
        }

        private void txtPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_regexDigits.IsMatch(((TextBox)sender).Text))
            {
                e.Handled = true;
                return;
            }

            btDone.IsEnabled = ConfigurationIsUpdated(txtHost, ((TextBox)sender), txtClientId);
        }

        private void txtClientId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_regexDigits.IsMatch(((TextBox)sender).Text))
            {
                e.Handled = true;
                return;
            }

            btDone.IsEnabled = ConfigurationIsUpdated(txtHost, txtPort, ((TextBox)sender));
        }

        private bool ConfigurationIsUpdated(TextBox hostTextBox, TextBox portTextBox, TextBox clientIdTextBox)
        {
            if(string.IsNullOrWhiteSpace(hostTextBox.Text)
                || string.IsNullOrWhiteSpace(portTextBox.Text)
                || string.IsNullOrWhiteSpace(clientIdTextBox.Text))
            {
                return false;
            }
            
            bool newHost = !string.Equals(_initialHost, hostTextBox.Text, StringComparison.InvariantCultureIgnoreCase);
            bool newPort = _initialPort != Convert.ToInt32(portTextBox.Text);
            bool newClientId = _initialClientId != Convert.ToInt32(clientIdTextBox.Text);
            
            return newHost || newPort || newClientId;
        }
    }
}

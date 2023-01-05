using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using SsbHedger.Configuration;

namespace SsbHedger
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        IConfiguration _configuration;

        Regex _regexDigits = new ("^[0-9]*$");

        public ConfigurationWindow(IConfiguration configuration)
        {
            InitializeComponent();
            
            _configuration = configuration;
            txtHost.Text = (string)_configuration.GetValue("Host");
            txtPort.Text = _configuration.GetValue("Port").ToString();
            txtClientId.Text = _configuration.GetValue("ClientId").ToString();
            txtUnderlyingSymbol.Text = (string)_configuration.GetValue("UnderlyingSymbol");
            txtSessionStart.Text = (string)_configuration.GetValue("SessionStart");
            txtSessionEnd.Text = (string)_configuration.GetValue("SessionEnd");
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

            bool newHost = !string.Equals((string)_configuration.GetValue("Host"), hostTextBox.Text, StringComparison.InvariantCultureIgnoreCase);
            bool newPort = (int)_configuration.GetValue("Port") != Convert.ToInt32(portTextBox.Text);
            bool newClientId = (int)_configuration.GetValue("ClientId") != Convert.ToInt32(clientIdTextBox.Text);
            
            return newHost || newPort || newClientId;
        }
    }
}

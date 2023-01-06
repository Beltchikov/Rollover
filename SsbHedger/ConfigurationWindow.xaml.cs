using System;
using System.Collections.Generic;
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

        Regex _regexDigits = new("^[0-9]*$");
        Regex _regexHoursAndMinutes = new("^[\\d|:]{1,5}$");
   
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
            Dictionary<string, TextBox> textBoxesDict = BuildDefaultTextBoxesDictionary();
            textBoxesDict["txtHost"] = (TextBox)sender;
            btDone.IsEnabled = ConfigurationIsUpdated(textBoxesDict);
        }

        private void txtPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_regexDigits.IsMatch(((TextBox)sender).Text))
            {
                UndoInput(e, (TextBox)sender);
                return;
            }

            Dictionary<string, TextBox> textBoxesDict = BuildDefaultTextBoxesDictionary();
            textBoxesDict["txtPort"] = (TextBox)sender;
            btDone.IsEnabled = ConfigurationIsUpdated(textBoxesDict);
        }

        private void txtClientId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_regexDigits.IsMatch(((TextBox)sender).Text))
            {
                UndoInput(e, (TextBox)sender);
                return;
            }

            Dictionary<string, TextBox> textBoxesDict = BuildDefaultTextBoxesDictionary();
            textBoxesDict["txtClientId"] = (TextBox)sender;
            btDone.IsEnabled = ConfigurationIsUpdated(textBoxesDict);
        }

        private void txtUnderlyingSymbol_TextChanged(object sender, TextChangedEventArgs e)
        {
            Dictionary<string, TextBox> textBoxesDict = BuildDefaultTextBoxesDictionary();
            textBoxesDict["txtUnderlyingSymbol"] = (TextBox)sender;
            btDone.IsEnabled = ConfigurationIsUpdated(textBoxesDict);
        }

        private void txtSessionStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_regexHoursAndMinutes.IsMatch(((TextBox)sender).Text))
            {
                UndoInput(e, (TextBox)sender);
                return;
            }

            Dictionary<string, TextBox> textBoxesDict = BuildDefaultTextBoxesDictionary();
            textBoxesDict["txtSessionStart"] = (TextBox)sender;
            btDone.IsEnabled = ConfigurationIsUpdated(textBoxesDict);
        }

        private void txtSessionEnd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_regexHoursAndMinutes.IsMatch(((TextBox)sender).Text))
            {
                UndoInput(e, (TextBox)sender);
                return;
            }

            Dictionary<string, TextBox> textBoxesDict = BuildDefaultTextBoxesDictionary();
            textBoxesDict["txtSessionEnd"] = (TextBox)sender;
            btDone.IsEnabled = ConfigurationIsUpdated(textBoxesDict);
        }

        private bool ConfigurationIsUpdated(Dictionary<string, TextBox> textBoxesDict)
        {
            if (string.IsNullOrWhiteSpace(textBoxesDict["txtHost"].Text)
                || string.IsNullOrWhiteSpace(textBoxesDict["txtPort"].Text)
                || string.IsNullOrWhiteSpace(textBoxesDict["txtClientId"].Text)
                || string.IsNullOrWhiteSpace(textBoxesDict["txtUnderlyingSymbol"].Text)
                || string.IsNullOrWhiteSpace(textBoxesDict["txtSessionStart"].Text)
                || string.IsNullOrWhiteSpace(textBoxesDict["txtSessionEnd"].Text))
            {
                return false;
            }

            bool newHost = !string.Equals((string)_configuration.GetValue("Host"), textBoxesDict["txtHost"].Text, StringComparison.InvariantCultureIgnoreCase);
            bool newPort = (int)_configuration.GetValue("Port") != Convert.ToInt32(textBoxesDict["txtPort"].Text);
            bool newClientId = (int)_configuration.GetValue("ClientId") != Convert.ToInt32(textBoxesDict["txtClientId"].Text);
            bool newUnderlyingSymbol = !string.Equals((string)_configuration.GetValue("UnderlyingSymbol"), textBoxesDict["txtUnderlyingSymbol"].Text, StringComparison.InvariantCultureIgnoreCase);
            bool newSessionStart = !string.Equals((string)_configuration.GetValue("SessionStart"), textBoxesDict["txtSessionStart"].Text, StringComparison.InvariantCultureIgnoreCase);
            bool newSessionEnd = !string.Equals((string)_configuration.GetValue("SessionEnd"), textBoxesDict["txtSessionEnd"].Text, StringComparison.InvariantCultureIgnoreCase);

            return newHost || newPort || newClientId || newUnderlyingSymbol || newSessionStart || newSessionEnd;
        }

        private Dictionary<string, TextBox> BuildDefaultTextBoxesDictionary()
        {
            return new Dictionary<string, TextBox>
            {
                {"txtHost", txtHost },
                {"txtPort", txtPort },
                {"txtClientId", txtClientId },
                {"txtUnderlyingSymbol", txtUnderlyingSymbol},
                {"txtSessionStart", txtSessionStart},
                {"txtSessionEnd", txtSessionEnd },
            };
        }

        private static void UndoInput(TextChangedEventArgs e, TextBox textBox)
        {
            if(textBox.Text.Length == 0)
            {
                e.Handled = true;
                return;
            }
            
            var currentPosition = textBox.SelectionStart - 1;
            textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1, 1);
            if (currentPosition < 0)
            {
                e.Handled = true;
                return;
            }

            textBox.Select(currentPosition, 0);
            e.Handled = true;
            return;
        }
    }
}

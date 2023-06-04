using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SsbHedger.Model;
using SsbHedger.SsbConfiguration;

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
        const string BORDER_BRUSH_HEX_COLOR = "#FFABADB3";


        public ConfigurationWindow(IConfiguration configuration)
        {
            InitializeComponent();

            _configuration = configuration;
            txtHost.Text = (string)_configuration.GetValue(Configuration.HOST);
            txtPort.Text = _configuration.GetValue(Configuration.PORT).ToString();
            txtClientId.Text = _configuration.GetValue(Configuration.CLIENT_ID).ToString();
            txtUnderlyingSymbol.Text = (string)_configuration.GetValue(Configuration.UNDERLYING_SYMBOL);
            txtSessionStart.Text = (string)_configuration.GetValue(Configuration.SESSION_START);
            txtSessionEnd.Text = (string)_configuration.GetValue(Configuration.SESSION_END);
            txtLastTradeDateOrContractMonth.Text = (string)_configuration.GetValue(Configuration.LAST_TRADE_DATE_OR_CONTRACT_MONTH);
            txtNumberOfStrikes.Text = Convert.ToString(_configuration.GetValue(Configuration.NUMBER_OF_STRIKES));

        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btDone_Click(object sender, RoutedEventArgs e)
        {
            if(!DateTime.TryParse(txtSessionStart.Text, out _))
            {
                txtSessionStart.BorderBrush = Brushes.Red;
                return;
            }

            if (!DateTime.TryParse(txtSessionEnd.Text, out _))
            {
                txtSessionEnd.BorderBrush = Brushes.Red;
                return;
            }

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
            txtSessionStart.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BORDER_BRUSH_HEX_COLOR));

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
            txtSessionStart.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BORDER_BRUSH_HEX_COLOR));

            if (!_regexHoursAndMinutes.IsMatch(((TextBox)sender).Text))
            {
                UndoInput(e, (TextBox)sender);
                return;
            }

            Dictionary<string, TextBox> textBoxesDict = BuildDefaultTextBoxesDictionary();
            textBoxesDict["txtSessionEnd"] = (TextBox)sender;
            btDone.IsEnabled = ConfigurationIsUpdated(textBoxesDict);
        }

        private void txtBearHedgeStrike_TextChanged(object sender, TextChangedEventArgs e)
        {
            double result;
            if(!double.TryParse(((TextBox)sender).Text, out result))
            {
                UndoInput(e, (TextBox)sender);
                return;
            }

            Dictionary<string, TextBox> textBoxesDict = BuildDefaultTextBoxesDictionary();
            textBoxesDict["txtLastTradeDateOrContractMonth"] = (TextBox)sender;
            btDone.IsEnabled = ConfigurationIsUpdated(textBoxesDict);
        }

        private void txtBullHedgeStrike_TextChanged(object sender, TextChangedEventArgs e)
        {
            double result;
            if (!double.TryParse(((TextBox)sender).Text, out result))
            {
                UndoInput(e, (TextBox)sender);
                return;
            }

            Dictionary<string, TextBox> textBoxesDict = BuildDefaultTextBoxesDictionary();
            textBoxesDict["txtBeullHedgeStrike"] = (TextBox)sender;
            btDone.IsEnabled = ConfigurationIsUpdated(textBoxesDict);
        }

        private bool ConfigurationIsUpdated(Dictionary<string, TextBox> textBoxesDict)
        {
            if (string.IsNullOrWhiteSpace(textBoxesDict["txtHost"].Text)
                || string.IsNullOrWhiteSpace(textBoxesDict["txtPort"].Text)
                || string.IsNullOrWhiteSpace(textBoxesDict["txtClientId"].Text)
                || string.IsNullOrWhiteSpace(textBoxesDict["txtUnderlyingSymbol"].Text)
                || string.IsNullOrWhiteSpace(textBoxesDict["txtSessionStart"].Text)
                || string.IsNullOrWhiteSpace(textBoxesDict["txtSessionEnd"].Text)
                || string.IsNullOrWhiteSpace(textBoxesDict["txtLastTradeDateOrContractMonth"].Text)
                || string.IsNullOrWhiteSpace(textBoxesDict["txtBullHedgeStrike"].Text))
            {
                return false;
            }

            bool newHost = !string.Equals((string)_configuration.GetValue(Configuration.HOST), 
                textBoxesDict["txtHost"].Text, StringComparison.InvariantCultureIgnoreCase);
            bool newPort = (int)_configuration.GetValue(Configuration.PORT) != Convert.ToInt32(textBoxesDict["txtPort"].Text);
            bool newClientId = (int)_configuration.GetValue(Configuration.CLIENT_ID) != Convert.ToInt32(textBoxesDict["txtClientId"].Text);
            bool newUnderlyingSymbol = !string.Equals((string)_configuration.GetValue(Configuration.UNDERLYING_SYMBOL), 
                textBoxesDict["txtUnderlyingSymbol"].Text, StringComparison.InvariantCultureIgnoreCase);
            bool newSessionStart = !string.Equals((string)_configuration.GetValue(Configuration.SESSION_START), 
                textBoxesDict["txtSessionStart"].Text, StringComparison.InvariantCultureIgnoreCase);
            bool newSessionEnd = !string.Equals((string)_configuration.GetValue(Configuration.SESSION_END), 
                textBoxesDict["txtSessionEnd"].Text, StringComparison.InvariantCultureIgnoreCase);
            bool lastTradeDateOrContractMonth = !string.Equals((string)_configuration.GetValue(Configuration.LAST_TRADE_DATE_OR_CONTRACT_MONTH),
                textBoxesDict["txtLastTradeDateOrContractMonth"].Text, StringComparison.InvariantCultureIgnoreCase);
            bool numberOfStrikes = !string.Equals(Convert.ToString(_configuration.GetValue(Configuration.NUMBER_OF_STRIKES), new CultureInfo("DE-de")),
                textBoxesDict["txtNumberOfStrikes"].Text, StringComparison.InvariantCultureIgnoreCase);

            return newHost || newPort || newClientId || newUnderlyingSymbol || newSessionStart || newSessionEnd
                || lastTradeDateOrContractMonth || numberOfStrikes;
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
                {"txtLastTradeDateOrContractMonth", txtLastTradeDateOrContractMonth },
                {"txtNumberOfStrikes", txtNumberOfStrikes },
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

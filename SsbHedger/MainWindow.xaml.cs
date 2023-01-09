using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
using System;
using System.Windows;

namespace SsbHedger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IConfiguration _configuration;

        public MainWindow(IConfiguration configuration)
        {
            InitializeComponent();
            _configuration = configuration;
        }

        private void btConfiguration_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow? _configurationWindow = new(_configuration);
            _configurationWindow.Owner= this;   

            bool ? configurationChanged = _configurationWindow.ShowDialog();
            if (configurationChanged == true)
            {
                object[] commandParams = new object[]
                {
                    _configurationWindow.txtHost.Text,
                    Convert.ToInt32(_configurationWindow.txtPort.Text),
                    Convert.ToInt32(_configurationWindow.txtClientId.Text),
                    _configurationWindow.txtUnderlyingSymbol.Text,
                    _configurationWindow.txtSessionStart.Text,
                    _configurationWindow.txtSessionEnd.Text,
                };
                ((MainWindowViewModel)DataContext).UpdateConfigurationCommand.Execute(commandParams);
            }
        }
    }
}

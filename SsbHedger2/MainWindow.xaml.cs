using SsbHedger.Model;
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
            DataContext = new MainWindowViewModel();
            _configuration = configuration;
        }

        private void btConfiguration_Click(object sender, RoutedEventArgs e)
        {
            var app = (App)Application.Current;
            ConfigurationWindow? _configurationWindow = new(_configuration);

            bool ? configurationChanged = _configurationWindow.ShowDialog();
            if (configurationChanged == true)
            {
                object[] commandParams = new object[]
                {
                    _configurationWindow.txtHost.Text,
                    Convert.ToInt32(_configurationWindow.txtPort.Text),
                    Convert.ToInt32(_configurationWindow.txtClientId.Text)
                };
                ((MainWindowViewModel)DataContext).UpdateConfigurationCommand.Execute(commandParams);
            }
        }
    }
}

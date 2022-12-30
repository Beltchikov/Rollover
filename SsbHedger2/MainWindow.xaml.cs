using SsbHedger2.Model;
using System;
using System.Windows;

namespace SsbHedger2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = ((MainWindowViewModel)DataContext);
        }

        private void btConfiguration_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow? _configurationWindow = new(
                    _viewModel.Host,
                    _viewModel.Port,
                    _viewModel.ClientId);

            bool? configurationChanged = _configurationWindow.ShowDialog();
            if (configurationChanged == true)
            {
                object[] commandParams = new object[]
                {
                    _viewModel,
                    _configurationWindow.txtHost.Text,
                    Convert.ToInt32(_configurationWindow.txtPort.Text),
                    Convert.ToInt32(_configurationWindow.txtClientId.Text)
                };
                _viewModel.UpdateConfigurationCommand.Execute(commandParams);
            }
        }
    }
}

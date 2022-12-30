using CommunityToolkit.Mvvm.Input;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btConfiguration_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow? _configurationWindow = new(
                    ((MainWindowViewModel)DataContext).Host,
                    ((MainWindowViewModel)DataContext).Port,
                    ((MainWindowViewModel)DataContext).ClientId);

            bool? configurationChanged = _configurationWindow.ShowDialog();
            if (configurationChanged == true)
            {
                var viewModel = ((MainWindowViewModel)DataContext);

                object[] commandParams = new object[]
                {
                    viewModel,
                    _configurationWindow.txtHost.Text,
                    Convert.ToInt32(_configurationWindow.txtPort.Text),
                    Convert.ToInt32(_configurationWindow.txtClientId.Text)
                };

                viewModel.UpdateConfigurationCommand.Execute(commandParams);
            }
        }
    }
}

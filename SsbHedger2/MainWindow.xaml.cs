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
                // TODO
                // UpdateConfigurationCommand

                //UpdateConfigurationCommand = new RelayCommand<string>(async (data) =>
                //{
                //    if (data == null) { throw new ApplicationException("Unexpected! data is null"); }
                //    var dataArray = data.Split(";").Select(m => m.Trim()).ToList();
                //    Host = dataArray[0];
                //    Port = Convert.ToInt32(dataArray[1]);
                //    ClientId = Convert.ToInt32(dataArray[2]);
                //    if (CloseAction == null) { throw new ApplicationException("Unexpected! CloseAction is null"); }
                //    await _mediator.Publish(new UpdateConfigurationMediatorCommand(
                //        Host, Port, ClientId, CloseAction));
                //});

                ((MainWindowViewModel)DataContext).Host = _configurationWindow.txtHost.Text;
                ((MainWindowViewModel)DataContext).Port = Convert.ToInt32(_configurationWindow.txtPort.Text);
                ((MainWindowViewModel)DataContext).ClientId = Convert.ToInt32(_configurationWindow.txtClientId.Text);
            }
        }
    }
}

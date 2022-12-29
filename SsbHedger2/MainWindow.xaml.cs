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
        ConfigurationWindow? _configurationWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btConfiguration_Click(object sender, RoutedEventArgs e)
        {
            if(_configurationWindow == null)
            {
                _configurationWindow = new(
                    ((MainWindowViewModel)DataContext).Host,
                    ((MainWindowViewModel)DataContext).Port,
                    ((MainWindowViewModel)DataContext).ClientId);
            }
            
            bool? configurationChanged = _configurationWindow.ShowDialog();
            if (configurationChanged == true)
            {
                ((MainWindowViewModel)DataContext).Host = _configurationWindow.txtHost.Text;
                ((MainWindowViewModel)DataContext).Port = Convert.ToInt32(_configurationWindow.txtPort.Text);
                ((MainWindowViewModel)DataContext).ClientId = Convert.ToInt32(_configurationWindow.txtClientId.Text);
            }
        }
    }
}

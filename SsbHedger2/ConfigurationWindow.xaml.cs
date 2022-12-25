using SsbHedger2.Model;
using System;
using System.Windows;

namespace SsbHedger2
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow(string host, int port, int clientId)
        {
            InitializeComponent();

            ((ConfigurationWindowViewModel)DataContext).Host = host;
            ((ConfigurationWindowViewModel)DataContext).Port = port;
            ((ConfigurationWindowViewModel)DataContext).ClientId = clientId;
            ((ConfigurationWindowViewModel)DataContext).CloseAction = new Action(Close);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

using SsbHedger.Model;
using System.Windows;

namespace SsbHedger
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
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

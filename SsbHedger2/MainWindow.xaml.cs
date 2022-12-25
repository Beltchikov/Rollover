using SsbHedger2.Model;
using System.Windows;

namespace SsbHedger2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ConfigurationWindow _configurationWindow;

        public MainWindow()
        {
            InitializeComponent();

            //((MainWindowViewModel)DataContext).Host = host;
            //((MainWindowViewModel)DataContext).Port = port;
            //((MainWindowViewModel)DataContext).ClientId = clientId;

            _configurationWindow = new("", 0, 0);
        }

        private void btConfiguration_Click(object sender, RoutedEventArgs e)
        {
            var configurationChanged = _configurationWindow.ShowDialog();
            if (configurationChanged == true)
            {
                //TODO Update view model
            }
        }
    }
}

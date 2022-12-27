using SsbHedger2.Model;
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
            
            var configurationChanged = _configurationWindow.ShowDialog();
            if (configurationChanged == true)
            {
                //TODO Update view model
            }
        }
    }
}

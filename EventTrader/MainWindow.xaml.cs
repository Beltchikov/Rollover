using System.Windows;

namespace EventTrader
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

        private void btTestUsdIr_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("USD ");
        }

        private void btStartSession_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

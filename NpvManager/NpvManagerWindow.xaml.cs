using NpvManager.Model;
using System.Windows;

namespace NpvManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class NpvManagerWindow : Window
    {
        public NpvManagerWindow()
        {
            InitializeComponent();
            DataContext = new NpvManagerViewModel();
        }
    }
}
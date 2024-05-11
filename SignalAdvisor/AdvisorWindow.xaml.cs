using SignalAdvisor.Model;
using System.Windows;

namespace SignalAdvisor
{
    /// <summary>
    /// Interaction logic for AdvisorWindow.xaml
    /// </summary>
    public partial class AdvisorWindow : Window
    {
        public AdvisorWindow()
        {
            InitializeComponent();
            DataContext = new AdvisorViewModel();
        }
    }
}

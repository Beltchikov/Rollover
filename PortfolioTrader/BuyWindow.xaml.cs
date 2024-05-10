using PortfolioTrader.Model;
using System.Windows;

namespace PortfolioTrader
{
    /// <summary>
    /// Interaction logic for BuyWindow.xaml
    /// </summary>
    public partial class BuyWindow : Window
    {
        public BuyWindow()
        {
            InitializeComponent();
            DataContext = new BuyViewModel();
        }
    }
}

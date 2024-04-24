using System.Configuration;
using System.Data;
using System.Windows;

namespace PortfolioTrader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static readonly int TIMEOUT = 1000;
        internal static readonly int MAX_BUY_SELL = 20;
    }

}

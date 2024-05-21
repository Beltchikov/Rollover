using System.Windows;

namespace PortfolioTrader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static readonly int TIMEOUT = 1000;
        internal static readonly int MAX_BUY_SELL = 40;
        internal static readonly string SEC_TYPE_STK = "STK";
        internal static readonly string USD = "USD";
        internal static readonly string EXCHANGE = "SMART";
        internal static readonly int MIN_ENTRY_BAR_IN_CENTS = 5;
    }
}

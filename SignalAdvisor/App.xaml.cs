using IbClient.IbHost;
using SignalAdvisor.Model;
using System.Windows;

namespace SignalAdvisor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static readonly int TIMEOUT = 1000;
        internal static readonly string SEC_TYPE_STK = "STK";
        internal static readonly string USD = "USD";
        internal static readonly string EXCHANGE = "SMART";
        internal static readonly string ACCOUNT_SUMMARY_TAGS = "AccountType,NetLiquidation,TotalCashValue,SettledCash,AccruedCash,BuyingPower,EquityWithLoanValue,PreviousEquityWithLoanValue,"
            + "GrossPositionValue,ReqTEquity,ReqTMargin,SMA,InitMarginReq,MaintMarginReq,AvailableFunds,ExcessLiquidity,Cushion,FullInitMarginReq,FullMaintMarginReq,FullAvailableFunds,"
            + "FullExcessLiquidity,LookAheadNextChange,LookAheadInitMarginReq ,LookAheadMaintMarginReq,LookAheadAvailableFunds,LookAheadExcessLiquidity,HighestSeverity,DayTradesRemaining,Leverage";
        internal static readonly int HISTORICAL_DATA_PERIOD_IN_HOURS = 72;
        internal static readonly string BAR_SIZE_STRING = "5 mins"; //https://interactivebrokers.github.io/tws-api/historical_bars.html#hd_duration

        internal static readonly string FORMAT_STRING_API = "yyyyMMdd-HH:mm:ss";
        internal static readonly int STD_DEV_PERIOD = 10;
        internal static readonly TimeOnly SESSION_START = TimeOnly.Parse("15:30");
        internal static readonly double RISK_IN_USD = 750;
        internal static readonly double LIVE_COST_PROFIT = 250;

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create instances of view model and IbHost
            AdvisorViewModel viewModel = new();
            IIbHost ibHost = new IbHost();
            viewModel.SetIbHost(ibHost);

            // Connect to TWS
            bool connected = false;
            await ibHost.ConnectAndStartReaderThread(
                viewModel.Host,
                viewModel.Port,
                viewModel.ClientId,
                (c) => { connected = c.IsConnected; },
                (ma) => { },
                (e) => { });

            while (!connected) { }

            // asign data context
            AdvisorWindow advisorWindow = new();
            advisorWindow.DataContext = viewModel;
            advisorWindow.Show();
        }
    }

}

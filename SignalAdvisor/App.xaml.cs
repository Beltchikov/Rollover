using System.Configuration;
using System.Data;
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
    }

}

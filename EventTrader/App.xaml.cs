using EventTrader.EconomicData;
using EventTrader.WebScraping;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace EventTrader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = new ServiceCollection()
                .AddSingleton<IBrowserWrapper, BrowserWrapper>()
                .AddSingleton<IEconomicDataTrader, EconomicDataTrader>()
                .BuildServiceProvider();
        }

        public IServiceProvider Services { get; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new(Services.GetRequiredService<IEconomicDataTrader>());
            mainWindow.Show();
        }
    }
}

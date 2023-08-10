using EventTrader.EconomicData;
using EventTrader.Requests;
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
                .AddSingleton<IInfiniteLoop, RequestLoop>()
                .AddSingleton<IEconDataLoop, EconDataLoop>()
                .AddSingleton<IDataProviderContext, DataProviderContext>()
                .BuildServiceProvider();
        }

        public IServiceProvider Services { get; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new()
            {
                DataContext = new ViewModel(Services.GetRequiredService<IEconDataLoop>())
            };
            mainWindow.Show();
        }
    }
}

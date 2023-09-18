using Dsmn.DataProviders;
using Dsmn.EconomicData;
using Dsmn.WebScraping;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace Dsmn
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
                .AddSingleton<IDataProviderContext, DataProviderContext>()
                .AddSingleton<IYahooProvider, YahooProvider>()
                .BuildServiceProvider();
        }

        public IServiceProvider Services { get; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new()
            {
                DataContext = new ViewModel(Services.GetRequiredService<IYahooProvider>())
            };
            mainWindow.Show();
        }
    }
}

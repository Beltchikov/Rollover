using Eomn.DataProviders;
using Eomn.Ib;
using Eomn.WebScraping;
using IBApi;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace Eomn
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
                .AddSingleton<IInvestingProvider, InvestingProvider>()
                .AddSingleton<IYahooProvider, YahooProvider>()
                .AddSingleton<ITwsProvider, TwsProvider>()
                .AddSingleton<IIbHost, IbHost>()
                .BuildServiceProvider();
        }

        public IServiceProvider Services { get; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new()
            {
                DataContext = new ViewModel(
                    Services.GetRequiredService<IInvestingProvider>(),
                    Services.GetRequiredService<IYahooProvider>(),
                    Services.GetRequiredService<ITwsProvider>(),
                    Services.GetRequiredService<IIbHost>())
            };
            mainWindow.Show();
        }
    }
}

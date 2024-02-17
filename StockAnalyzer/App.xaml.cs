using IbClient.IbHost;
using Microsoft.Extensions.DependencyInjection;
using StockAnalyzer.DataProviders;
using StockAnalyzer.WebScraping;
using System;
using System.Windows;

namespace StockAnalyzer
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
                .AddSingleton<IIbHostQueue, IbHostQueue>()
                .AddSingleton<ISeekingAlphaProvider, SeekingAlphaProvider>()
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
                    Services.GetRequiredService<ISeekingAlphaProvider>(),
                    Services.GetRequiredService<IIbHost>())
            };
            mainWindow.Show();
        }
    }
}

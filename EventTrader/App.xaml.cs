using Dsmn.DataProviders;
using Dsmn.Ib;
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
                .AddSingleton<IYahooProvider, YahooProvider>()
                .AddSingleton<IIbHost, IbHost>()
                .BuildServiceProvider();
        }

        public IServiceProvider Services { get; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new()
            {
                DataContext = new ViewModel(
                    Services.GetRequiredService<IYahooProvider>(),
                    Services.GetRequiredService<IIbHost>())
            };
            mainWindow.Show();
        }
    }
}

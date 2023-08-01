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
                .AddSingleton<IWebScraper, WebScraper>()
                .BuildServiceProvider();
        }

        public IServiceProvider Services { get; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new(Services.GetRequiredService<IWebScraper>());
            mainWindow.Show();
        }
    }
}

using Dsmn.DataProviders;
using Dsmn.EconomicData;
using Dsmn.Requests;
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
                .AddSingleton<IInfiniteLoop, RequestLoop>()
                .AddSingleton<IEconDataLoop, EconDataLoop>()
                .AddSingleton<IBrowserWrapper, BrowserWrapper>()
                .AddSingleton<IDataProviderContext, DataProviderContext>()
                .AddSingleton<IInvestingDataProvider, InvestingDataProvider>()
                .BuildServiceProvider();
        }

        public IServiceProvider Services { get; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new()
            {
                DataContext = new ViewModel(Services.GetRequiredService<IEconDataLoop>(),
                                            Services.GetRequiredService<IInvestingDataProvider>())
            };
            mainWindow.Show();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace SsbHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        public App()
        {
            Services = new ServiceCollection()
                //.AddSingleton<IBrowserWrapper, BrowserWrapper>()
                .BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
        }
    }
}

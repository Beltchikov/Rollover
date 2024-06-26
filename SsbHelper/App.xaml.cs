﻿using IbClient.IbHost;
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
                .AddSingleton<IIbHostQueue, IbHostQueue>()
                .AddSingleton<IIbHost, IbHost>()
                .BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new()
            {
                DataContext = new ViewModel(Services.GetRequiredService<IIbHost>())
            };
            mainWindow.Show();
        }
    }
}

using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SsbHedger.Abstractions;
using SsbHedger.CommandHandler;
using SsbHedger.Configuration;
using SsbHedger.Model;
using SsbHedger.RegistryManager;
using System;
using System.Windows;

namespace SsbHedger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = new ServiceCollection()
                .AddSingleton<IRegistryCurrentUserAbstraction, RegistryCurrentUserAbstraction>()
                .AddScoped<IRegistryKeyAbstraction, RegistryKeyAbstraction>()
                .AddSingleton<IConfiguration, Configuration.Configuration>()
                .AddSingleton<IRegistryManager, RegistryManager.RegistryManager>()
                .AddSingleton<IIbHost, IbHost>()
                .AddSingleton<IInitializeCommandHandler, InitializeCommandHandler>()
                .AddSingleton<IUpdateConfigurationCommandHandler, UpdateConfigurationCommandHandler>()
                .AddMediatR(GetType().Assembly)
                .BuildServiceProvider();
        }
        public IServiceProvider Services { get; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new(Services.GetRequiredService<IConfiguration>())
            {
                DataContext = new MainWindowViewModel(
                    Services.GetRequiredService<IInitializeCommandHandler>(),
                    Services.GetRequiredService<IUpdateConfigurationCommandHandler>())
            };
            mainWindow.Show();
        }
    }
}

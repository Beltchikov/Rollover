using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SsbHedger.Abstractions;
using SsbHedger.CommandHandler;
using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
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
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<IRegistryManager, RegistryManager>()
                .AddSingleton<IIbHost, IbHost>()
                .AddSingleton<IInitializeCommandHandler, InitializeCommandHandler>()
                .AddSingleton<IUpdateConfigurationCommandHandler, UpdateConfigurationCommandHandler>()
                .AddMediatR(GetType().Assembly)
                .BuildServiceProvider();
        }
        public IServiceProvider Services { get; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var viewModel = new MainWindowViewModel(
                    Services.GetRequiredService<IInitializeCommandHandler>(),
                    Services.GetRequiredService<IUpdateConfigurationCommandHandler>());

            var _registryManager = Services.GetRequiredService<IRegistryManager>();
            var _configuration = Services.GetRequiredService<IConfiguration>();
            var configurationdata = _registryManager.ReadConfiguration(new ConfigurationData(
                (string)_configuration.GetValue(Configuration.HOST),
                (int)_configuration.GetValue(Configuration.PORT),
                (int)_configuration.GetValue(Configuration.CLIENT_ID),
                (string)_configuration.GetValue(Configuration.UNDERLYING_SYMBOL),
                (string)_configuration.GetValue(Configuration.SESSION_START),
                (string)_configuration.GetValue(Configuration.SESSION_END)));

            _configuration.SetValue(Configuration.HOST, configurationdata.Host);
            _configuration.SetValue(Configuration.PORT, configurationdata.Port);
            _configuration.SetValue(Configuration.CLIENT_ID, configurationdata.ClientId);
            _configuration.SetValue(Configuration.UNDERLYING_SYMBOL, configurationdata.UnderlyingSymbol);
            _configuration.SetValue(Configuration.SESSION_START, configurationdata.SessionStart);
            _configuration.SetValue(Configuration.SESSION_END, configurationdata.SessionEnd);

            viewModel.SessionStart= configurationdata.SessionStart; 
            viewModel.SessionEnd= configurationdata.SessionEnd; 

            MainWindow mainWindow = new(_configuration)
            {
                DataContext = viewModel
            };
            mainWindow.Show();
        }
    }
}

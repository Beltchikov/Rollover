using Microsoft.Extensions.DependencyInjection;
using SsbHedger.Abstractions;
using SsbHedger.Builders;
using System;
using System.Windows;

namespace SsbHedger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IRegistryManager? _registryManager;
        private IMainWindowBuilder? _mainWindowBuilder;

        private readonly string _defaultHost = "localhost";
        private readonly int _defaultPort = 4001;
        private readonly int _defaultClientId = 3;

        public App()
        {
            Services = new ServiceCollection()
                .AddSingleton<IRegistryManager, RegistryManager>()
                .AddSingleton<IMainWindowBuilder, MainWindowBuilder>()
                .AddScoped<IRegistryCurrentUserAbstraction, RegistryCurrentUserAbstraction>()
                .BuildServiceProvider();
        }

        public IServiceProvider Services { get; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _registryManager = Services.GetService<IRegistryManager>();
            if(_registryManager == null ) 
            {
                throw new ApplicationException("Unexpected! _registryManager is null");
            }

            var (host, port, clientId) = _registryManager.ReadConfiguration(
                 _defaultHost,
                 _defaultPort,
                 _defaultClientId);

            _mainWindowBuilder = Services.GetService<IMainWindowBuilder>();
            if (_mainWindowBuilder == null)
            {
                throw new ApplicationException("Unexpected! _mainWindowBuilder is null");
            }
            MainWindow mainWindow = _mainWindowBuilder.Build(host, port, clientId);
            mainWindow?.Show();
        }
    }
}

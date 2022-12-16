using System.Windows;
using SsbHedger.Builders;

namespace SsbHedger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IRegistryManager _registryManager = new RegistryManager();
        private readonly IMainWindowBuilder _mainWindowBuilder = new MainWindowBuilder();

        private readonly string _defaultHost = "localhost";
        private readonly int _defaultPort = 4001;
        private readonly int _defaultClientId = 3;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
           var (host, port, clientId) = _registryManager.ReadConfiguration(
                _defaultHost,
                _defaultPort,
                _defaultClientId);

            MainWindow mainWindow = _mainWindowBuilder.Build(host, port, clientId);
            mainWindow?.Show();
        }
    }
}

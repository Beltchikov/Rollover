using System.Windows;
using SsbHedger.Builders;

namespace SsbHedger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        IRegistryManager _registryManager = new RegistryManager();
        private IMainWindowBuilder _mainWindowBuilder = new MainWindowBuilder();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var defaultHost = "localhost";
            int defaultPort = 4001;
            int defaultClientId = 1;

            var (host, port, clientId) = _registryManager.ReadConfiguration(
                defaultHost,
                defaultPort,
                defaultClientId);

            MainWindow mainWindow = _mainWindowBuilder.Build(host, port, clientId);
            mainWindow?.Show();
        }
    }
}

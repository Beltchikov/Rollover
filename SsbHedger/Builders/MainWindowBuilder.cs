namespace SsbHedger.Builders
{
    public class MainWindowBuilder : IMainWindowBuilder
    {
        public MainWindow Build(string host, int port, int clientId)
        {
            return new MainWindow(host, port, clientId);
        }
    }
}

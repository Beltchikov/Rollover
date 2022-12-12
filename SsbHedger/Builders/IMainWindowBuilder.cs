namespace SsbHedger.Builders
{
    public interface IMainWindowBuilder
    {
        MainWindow Build(string host, int port, int clientId);
    }
}
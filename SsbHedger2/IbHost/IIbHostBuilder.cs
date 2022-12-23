using SsbHedger2.Model;

namespace SsbHedger2.IbHost
{
    internal interface IIbHostBuilder
    {
        IIbHost Build(MainWindowViewModel viewModel, string host, int port, int clientId);
    }
}
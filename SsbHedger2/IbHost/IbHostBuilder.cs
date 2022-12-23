using SsbHedger2.Model;

namespace SsbHedger2.IbHost
{
    internal class IbHostBuilder : IIbHostBuilder
    {
        public IIbHost Build(MainWindowViewModel viewModel, string host, int port, int clientId)
        {
            return new IbHost(viewModel, host, port, clientId);
        }
    }
}
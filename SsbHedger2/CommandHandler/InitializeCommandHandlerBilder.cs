using SsbHedger2.IbHost;
using SsbHedger2.RegistryManager;

namespace SsbHedger2.CommandHandler
{
    internal class InitializeCommandHandlerBilder : IInitializeCommandHandlerBilder
    {
        public InitializeCommandHandler Build(IRegistryManager registryManager, IIbHost ibHost)
        {
            return InitializeCommandHandler.Create(registryManager, ibHost);
        }
    }
}

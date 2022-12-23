using SsbHedger2.IbHost;
using SsbHedger2.RegistryManager;

namespace SsbHedger2.CommandHandler
{
    internal interface IInitializeCommandHandlerBilder
    {
        InitializeCommandHandler Build(IRegistryManager registryManager, IIbHost ibHost);
    }
}
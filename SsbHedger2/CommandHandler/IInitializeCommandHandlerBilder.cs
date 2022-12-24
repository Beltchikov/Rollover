using SsbHedger2.Model;

namespace SsbHedger2.CommandHandler
{
    internal interface IInitializeCommandHandlerBilder
    {
        InitializeCommandHandler Build(MainWindowViewModel mainWindowViewModel);
    }
}
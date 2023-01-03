using SsbHedger.Model;

namespace SsbHedger.CommandHandler
{
    public interface IUpdateConfigurationCommandHandler
    {
        void Handle(MainWindowViewModel viewModel, object[] parameters);
    }
}
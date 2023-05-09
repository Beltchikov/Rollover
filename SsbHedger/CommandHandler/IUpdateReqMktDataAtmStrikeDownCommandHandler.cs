using SsbHedger.Model;

namespace SsbHedger.CommandHandler
{
    public interface IUpdateReqMktDataAtmStrikeDownCommandHandler
    {
        void Handle(MainWindowViewModel viewModel, object[] parameters);
    }
}

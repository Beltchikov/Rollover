using SsbHedger.Model;

namespace SsbHedger.CommandHandler
{
    public interface IUpdateReqMktDataAtmStrikeCallCommandHandler
    {
        void Handle(MainWindowViewModel viewModel, object[] parameters);
    }
}

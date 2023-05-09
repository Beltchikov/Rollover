using SsbHedger.Model;

namespace SsbHedger.CommandHandler
{
    public interface IUpdateReqMktDataAtmStrikeUpCommandHandler
    {
        void Handle(MainWindowViewModel viewModel, object[] parameters);
    }
}

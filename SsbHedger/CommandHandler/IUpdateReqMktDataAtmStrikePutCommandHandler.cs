using SsbHedger.Model;

namespace SsbHedger.CommandHandler
{
    public interface IUpdateReqMktDataAtmStrikePutCommandHandler
    {
        void Handle(MainWindowViewModel viewModel, object[] parameters);
    }
}

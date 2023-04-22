using System.Threading.Tasks;

namespace SsbHedger.CommandHandler
{
    public interface IDeltaAlertActivateCommandHandler
    {
        void Handle(Model.MainWindowViewModel mainWindowViewModel, bool activate);
    }
}
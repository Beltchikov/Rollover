using System.Threading.Tasks;

namespace SsbHedger.CommandHandler
{
    public interface IDeltaAlertActivateCommandHandler
    {
        Task HandleAsync(Model.MainWindowViewModel mainWindowViewModel, bool activate);
    }
}
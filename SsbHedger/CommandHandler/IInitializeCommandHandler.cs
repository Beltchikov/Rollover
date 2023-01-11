using System.Threading.Tasks;

namespace SsbHedger.CommandHandler
{
    public interface IInitializeCommandHandler
    {
        Task HandleAsync(Model.MainWindowViewModel mainWindowViewModel);
    }
}
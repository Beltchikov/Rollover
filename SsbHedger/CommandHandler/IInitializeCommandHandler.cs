namespace SsbHedger.CommandHandler
{
    public interface IInitializeCommandHandler
    {
        void Handle(Model.MainWindowViewModel mainWindowViewModel);
    }
}
namespace SsbHedger.CommandHandler
{
    public interface IVolatilityAlertActivateCommandHandler
    {
        void Handle(Model.MainWindowViewModel mainWindowViewModel, object[] parameters);
    }
}

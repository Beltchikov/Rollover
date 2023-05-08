namespace SsbHedger.CommandHandler
{
    public interface IFindAtmStrikesCommandHandler
    {
        void Handle(Model.MainWindowViewModel mainWindowViewModel, object[] parameters);
    }
}

namespace SsbHedger.CommandHandler
{
    public interface IFindStrikesCommandHandler
    {
        void Handle(Model.MainWindowViewModel mainWindowViewModel, object[] parameters);
    }
}

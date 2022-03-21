namespace UsMoversOpening
{
    public interface IThreadSpawner
    {
        bool ExitFlagInputThread { get; set; }

        void Run();
    }
}
namespace UsMoversOpening.Threading
{
    public interface IThreadSpawner
    {
        bool ExitFlagInputThread { get; set; }

        void Run();
    }
}
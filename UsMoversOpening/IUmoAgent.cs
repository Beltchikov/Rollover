namespace UsMoversOpening
{
    public interface IUmoAgent
    {
        void Run(IThreadSpawner threadSpawner, IThreadWrapper inputThread);
    }
}
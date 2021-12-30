namespace Rollover
{
    public interface IRolloverAgent
    {
        public void Run();
        public void Connect();
        public void ListPositions();
        public void RunInputLoop();
    }
}
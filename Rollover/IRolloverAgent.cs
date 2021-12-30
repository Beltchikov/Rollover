using Rollover.Ib;

namespace Rollover
{
    public interface IRolloverAgent
    {
        IRepository Repository { get; }

        public void Run();
    }
}
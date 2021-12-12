namespace Rollover.Ib
{
    public interface IRequestSender
    {
        void RegisterResponseHandlers();
        void Connect();
    }
}